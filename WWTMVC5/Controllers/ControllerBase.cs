//-----------------------------------------------------------------------
// <copyright file="ControllerBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Live;
using Newtonsoft.Json;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Services;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.WebServices;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Base class for all controller which require the Live ID authentication functionality.
    /// </summary>
    public class ControllerBase : Controller
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ControllerBase class.
        /// </summary>
        /// <param name="profileService">Instance of profile Service</param>
        public ControllerBase(IProfileService profileService)
        {
            ProfileService = profileService;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the current user ID
        /// </summary>
        public long CurrentUserId
        {
            get
            {
                return SessionWrapper.Get<long?>("CurrentUserID") ?? 0;
            }
        }

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
                            SessionWrapper.Set<bool>("IsSiteAdmin", false);
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

        protected async Task<LiveLoginResult> TryAuthenticateFromHttpContext(ICommunityService communityService, INotificationService notificationService)
        {
            var svc = new LiveIdAuth();
            var result = await svc.Authenticate();
            if (result.Status == LiveConnectSessionStatus.Connected)
            {
                var client = new LiveConnectClient(result.Session);
                SessionWrapper.Set("LiveConnectClient", client);
                SessionWrapper.Set("LiveConnectResult", result);
                SessionWrapper.Set("LiveAuthSvc", svc);

                var getResult = await client.GetAsync("me");
                var jsonResult = getResult.Result as dynamic;
                var profileDetails = ProfileService.GetProfile(jsonResult.id);
                if (profileDetails == null)
                {
                    profileDetails = new ProfileDetails(jsonResult);
                    // While creating the user, IsSubscribed to be true always.
                    profileDetails.IsSubscribed = true;

                    // When creating the user, by default the user type will be of regular. 
                    profileDetails.UserType = UserTypes.Regular;
                    profileDetails.ID = ProfileService.CreateProfile(profileDetails);

                    // This will used as the default community when user is uploading a new content.
                    // This community will need to have the following details:
                    var communityDetails = new CommunityDetails
                    {
                        CommunityType = CommunityTypes.User, // 1. This community type should be User
                        CreatedByID = profileDetails.ID, // 2. CreatedBy will be the new USER.
                        IsFeatured = false, // 3. This community is not featured.
                        Name = Resources.UserCommunityName, // 4. Name should be NONE.
                        AccessTypeID = (int) AccessType.Private, // 5. Access type should be private.
                        CategoryID = (int) CategoryType.GeneralInterest
                        // 6. Set the category ID of general interest. We need to set the Category ID as it is a foreign key and cannot be null.
                    };

                    // 7. Create the community
                    communityService.CreateCommunity(communityDetails);

                    // Send New user notification.
                    notificationService.NotifyNewEntityRequest(profileDetails,
                        HttpContext.Request.Url.GetServerLink());
                }

                SessionWrapper.Set<long>("CurrentUserID", profileDetails.ID);
                SessionWrapper.Set<string>("CurrentUserProfileName",
                    profileDetails.FirstName + " " + profileDetails.LastName);
                SessionWrapper.Set("ProfileDetails", profileDetails);
                SessionWrapper.Set("AuthenticationToken", result.Session.AuthenticationToken);
            }
            return result;
        }

        /// <summary>
        /// Checks if the user is site admin or not
        /// No DB check as Most of the users are not site admins and will have session value as false
        /// </summary>
        protected static void CheckIfSiteAdmin()
        {
            if (!SessionWrapper.Get<bool>("IsSiteAdmin", false))
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
                pageName = HttpContext.Request.UrlReferrer.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }
            else
            {
                pageName = HttpContext.Request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }

            if (highlightType == HighlightType.None)
            {
                ViewData["PrefixId"] = string.Format(CultureInfo.InvariantCulture, "{0}_", pageName);
            }
            else
            {
                ViewData["PrefixId"] = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_", pageName, highlightType.ToString());
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
                token = System.Web.HttpContext.Current.Request.QueryString["LiveUserToken"];
            }
            var cachedProfile = ProfileCacheManager.GetProfileDetails(token);
            if (cachedProfile!=null)
            {
                return cachedProfile;
            }
            var userId = await svc.GetUserId(token);
            
            if (userId != null && userId.Length > 3)
            {
                var profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
                var profileDetails = profileService.GetProfile(userId);
                if (profileDetails != null)
                {
                    ProfileCacheManager.CacheProfile(token,profileDetails);
                }
                
                return profileDetails;
            }
            
            return null;
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