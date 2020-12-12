//-----------------------------------------------------------------------
// <copyright file="ControllerBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Live;
using Newtonsoft.Json;
using Unity;
using WWT.Providers;

using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.WebServices;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Base class for all controller which require the Live ID authentication functionality.
    /// </summary>
    public class ControllerBase : Controller
    {
        private readonly ILogger<ControllerBase> _logger;
        protected readonly IExternalUrlInfo _urlInfo;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ControllerBase class.
        /// </summary>
        /// <param name="profileService">Instance of profile Service</param>
        public ControllerBase(IProfileService profileService)
        {
            ProfileService = profileService;
            _logger = UnityConfig.Container.Resolve<ILogger<ControllerBase>>();
            _urlInfo = UnityConfig.Container.Resolve<IExternalUrlInfo>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current user ID
        /// </summary>
        public long CurrentUserId => SessionWrapper.Get<long?>("CurrentUserID") ?? 0;

        /// <summary>
        /// Gets a value indicating whether the current user is site admin or not
        /// </summary>
        public bool IsSiteAdmin
        {
            get
            {
                var isSiteAdmin = SessionWrapper.Get<bool?>("IsSiteAdmin");

                if (!isSiteAdmin.HasValue)
                {
                    var identity = HttpContext.GetIdentityName();
                    if (!string.IsNullOrWhiteSpace(identity))
                    {
                        var profile = ProfileService.GetProfile(identity);
                        if (profile != null)
                        {
                            isSiteAdmin = (profile.UserType == UserTypes.SiteAdmin) ? true : false;
                            profile.SetProfileSessionValues();
                        }
                        else
                        {
                            // If the user is signed but not accepted TOC, profile will not be created. In that case, repeated call
                            // to the DB to be avoided.
                            SessionWrapper.Set("IsSiteAdmin", false);
                        }
                    }
                }

                return (isSiteAdmin ?? false);
            }
        }



        /// <summary>
        /// Gets or sets Instance of profile Service
        /// </summary>
        protected IProfileService ProfileService { get; set; }

        #endregion Properties

        #region Protected Methods

        protected async Task<ProfileDetails> TryAuthenticateFromAuthCode(string authCode)
        {
            if (SessionWrapper.Get<ProfileDetails>("ProfileDetails") != null)
            {
                return SessionWrapper.Get<ProfileDetails>("ProfileDetails");
            }

            var svc = new LiveIdAuth();
            var profile = await TryRefreshToken(svc);

            if (profile == null && authCode.Length > 1)
            {
                profile = await UserFromToken(await svc.GetTokens(authCode), svc);
            }

            return profile;
        }

        private async Task<ProfileDetails> UserFromToken(string tokenResult, LiveIdAuth svc)
        {
            if (string.IsNullOrEmpty(tokenResult))
            {
                return null;
            }

            var tokens = new { access_token = "", refresh_token = "" };
            var json = JsonConvert.DeserializeAnonymousType(tokenResult, tokens);

            if (string.IsNullOrEmpty(json.access_token) || string.IsNullOrEmpty(json.refresh_token)) {
                _logger.LogWarning("UserFromToken failed: {reply}", tokenResult);
                return null;
            }

            var userId = await svc.GetUserId(json.access_token);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return await InitUserProfile(userId, json.access_token);
        }

        private async Task<ProfileDetails> TryRefreshToken(LiveIdAuth svc)
        {
            if (Request.Cookies["refresh_token"] != null && Request.Cookies["refresh_token"].Value.Length > 1)
            {
                var result = await svc.RefreshTokens();
                return await UserFromToken(result, svc);
            }
            return null;
        }

        private async Task<ProfileDetails> InitUserProfile(string liveId, string accessToken)
        {
            if (string.IsNullOrEmpty(liveId))
            {
                return null;
            }

            var profileDetails = ProfileService.GetProfile(liveId);

            if (profileDetails == null)
            {
                if (string.IsNullOrEmpty(accessToken))
                {
                    return null;
                }

                var svc = new LiveIdAuth();
                var jsonResult = await svc.GetMeInfo(accessToken);

                profileDetails = new ProfileDetails(jsonResult)
                {
                    IsSubscribed = true,
                    UserType = UserTypes.Regular
                };
                profileDetails.ID = ProfileService.CreateProfile(profileDetails);

                // This will used as the default community when user is uploading a new content.
                // This community will need to have the following details:
                var communityDetails = new CommunityDetails
                {
                    CommunityType = CommunityTypes.User, // 1. This community type should be User
                    CreatedByID = profileDetails.ID, // 2. CreatedBy will be the new USER.
                    IsFeatured = false, // 3. This community is not featured.
                    Name = Resources.UserCommunityName, // 4. Name should be NONE.
                    AccessTypeID = (int)AccessType.Private, // 5. Access type should be private.
                    CategoryID = (int)CategoryType.GeneralInterest
                };

                var communityService = DependencyResolver.Current.GetService(typeof(ICommunityService)) as ICommunityService;
                var notificationService = DependencyResolver.Current.GetService(typeof(INotificationService)) as INotificationService;

                // 7. Create the community
                communityService.CreateCommunity(communityDetails);

                // Send New user notification.
                notificationService.NotifyNewEntityRequest(profileDetails, HttpContext.Request.Url.GetServerLink());
            }

            SessionWrapper.Set("CurrentUserID", profileDetails.ID);
            SessionWrapper.Set("CurrentUserProfileName", profileDetails.FirstName + " " + profileDetails.LastName);
            SessionWrapper.Set("ProfileDetails", profileDetails);
            return profileDetails;
        }

        protected async Task<ProfileDetails> TryAuthenticateFromHttpContext()
        {
            if (SessionWrapper.Get<ProfileDetails>("ProfileDetails") != null)
            {
                return SessionWrapper.Get<ProfileDetails>("ProfileDetails");
            }

            var svc = new LiveIdAuth();

            var profile = await TryRefreshToken(svc);
            if (profile != null)
            {
                return profile;
            }

            var result = await svc.Authenticate();
            if (result.Status != LiveConnectSessionStatus.Connected)
            {
                return await UserFromToken(await svc.RefreshTokens(), svc);
            }

            var client = new LiveConnectClient(result.Session);
            var getMeResult = await client.GetAsync("me");
            string userId = null;

            foreach (KeyValuePair<string,object> item in getMeResult.Result)
            {
                if (item.Key.ToLower() == "id")
                {
                    userId = item.Value.ToString();
                }
            }

            return await InitUserProfile(userId, result.Session.AccessToken);
        }

        /// <summary>
        /// Checks if the user is site admin or not
        /// No DB check as Most of the users are not site admins and will have session value as false
        /// </summary>
        protected static void CheckIfSiteAdmin()
        {
            if (!SessionWrapper.Get("IsSiteAdmin", false))
            {
                throw new HttpException(401, Resources.NoPermissionAdminScreenMessage);
            }
        }

        /// <summary>
        /// It creates the prefix for id of links
        /// </summary>
        /// <param name="highlightType">Related / Latest / Top etc.</param>
        protected void SetSiteAnalyticsPrefix(HighlightType highlightType)
        {
            var pageName = string.Empty;

            if (HttpContext.Request.IsAjaxRequest())
            {
                pageName = HttpContext.Request.UrlReferrer.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
            else
            {
                pageName = HttpContext.Request.Url.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }

            if (highlightType == HighlightType.None)
            {
                ViewData["PrefixId"] = string.Format(CultureInfo.InvariantCulture, "{0}_", pageName);
            }
            else
            {
                ViewData["PrefixId"] = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_", pageName, highlightType);
            }
        }

        protected static long ValidateEntityId(string id)
        {
            long result = 0;
            if (!long.TryParse(id, out result))
            {
                throw new WebFaultException<string>("Invalid ID", HttpStatusCode.BadRequest);
            }

            return result;
        }

        protected static async Task<ProfileDetails> ValidateAuthentication()
        {
            var svc = new LiveIdAuth();

            var token = System.Web.HttpContext.Current.Request.Headers["LiveUserToken"];
            if (token == null)
            {
                var authCookie = System.Web.HttpContext.Current.Request.Cookies["access_token"];
                if (authCookie != null)
                {
                    token = authCookie.Value;
                }
            }

            if (string.IsNullOrEmpty(token))
                return null;

            var cachedProfile = ProfileCacheManager.GetProfileDetails(token);
            if (cachedProfile != null)
            {
                return cachedProfile;
            }

            string userId = await svc.GetUserId(token);

            if (userId == null)
            {
                var result = await svc.RefreshTokens();
                var tokens = new { access_token = "", refresh_token = "" };
                var json = JsonConvert.DeserializeAnonymousType(result, tokens);

                if (string.IsNullOrEmpty(json.access_token))
                    return null;

                userId = await svc.GetUserId(json.access_token);
            }

            if (userId == null || userId.Length <= 3)
                return null;

            var profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
            var profileDetails = profileService.GetProfile(userId);
            if (profileDetails != null && token != null)
            {
                ProfileCacheManager.CacheProfile(token, profileDetails);
            }

            return profileDetails;
        }

        protected static Guid ValidateGuid(string guid)
        {
            var result = Guid.Empty;
            if (!Guid.TryParse(guid, out result))
            {
                throw new WebFaultException<string>("Invalid GUID", HttpStatusCode.BadRequest);
            }

            return result;
        }
        #endregion Protected Methods
    }
}