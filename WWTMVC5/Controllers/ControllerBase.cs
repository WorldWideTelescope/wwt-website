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
            this.ProfileService = profileService;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the current user ID
        /// </summary>
        public long CurrentUserID
        {
            get
            {
                long? userID = SessionWrapper.Get<long?>("CurrentUserID");

                //if (!userID.HasValue)
                //{
                //    var identity = HttpContext.GetIdentityName();
                //    if (!string.IsNullOrWhiteSpace(identity))
                //    {
                //        var profile = ProfileService.GetProfile(identity);
                //        if (profile != null)
                //        {
                //            userID = profile.ID;
                //            profile.SetProfileSessionValues();
                //        }
                //        else
                //        {
                //            // If the user is signed but not accepted TOC, profile will not be created. In that case, repeated call
                //            // to the DB to be avoided.
                //            SessionWrapper.Set<long>("CurrentUserID", 0);
                //        }
                //    }
                //}

                return (userID ?? 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current user is site admin or not
        /// </summary>
        public bool IsSiteAdmin
        {
            get
            {
                bool? isSiteAdmin = SessionWrapper.Get<bool?>("IsSiteAdmin");

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
        /// Gets the current user's profile name
        /// </summary>
        public string CurrentUserProfileName
        {
            get
            {
                string userProfileName = SessionWrapper.Get<string>("CurrentUserProfileName", null);

                if (userProfileName == null)
                {
                    var identity = HttpContext.GetIdentityName();
                    if (!string.IsNullOrWhiteSpace(identity))
                    {
                        var profile = ProfileService.GetProfile(identity);
                        if (profile != null)
                        {
                            userProfileName = profile.GetProfileName();
                            profile.SetProfileSessionValues();
                        }
                        else
                        {
                            userProfileName = HttpContext.GetIdentityProfileName();
                            SessionWrapper.Set<string>("CurrentUserProfileName", userProfileName);
                        }
                    }
                }

                return userProfileName ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets Instance of profile Service
        /// </summary>
        protected IProfileService ProfileService { get; set; }

        #endregion Properties

        #region Protected Methods

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
            string pageName = string.Empty;

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
            var cachedProfile = ProfileCacheManager.GetProfileDetails(token);
            if (cachedProfile!=null)
            {
                return cachedProfile;
            }
            string userId = await svc.GetUserId(token);
            
            if (userId != null && userId.Length > 3)
            {
                IProfileService profileService = DependencyResolver.Current.GetService(typeof(IProfileService)) as IProfileService;
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
            Guid result = Guid.Empty;
            if (!Guid.TryParse(guid, out result))
            {
                throw new WebFaultException<string>("Invalid GUID", HttpStatusCode.BadRequest);
            }

            return result;
        }
        #endregion Protected Methods
    }
}