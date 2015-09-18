//-----------------------------------------------------------------------
// <copyright file="ProfileController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Live;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the Profile related requests.
    /// </summary>
    public class ProfileController : ControllerBase
    {
        /// <summary>
        /// Instance of community Service
        /// </summary>
        private ICommunityService _communityService;

        /// <summary>
        /// Instance of Queue Service
        /// </summary>
        private INotificationService _notificationService;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ProfileController class.
        /// </summary>
        /// <param name="profileService">Instance of profile Service</param>
        /// <param name="communityService">Instance of community Service</param>
        public ProfileController(IProfileService profileService, ICommunityService communityService, INotificationService queueService)
            : base(profileService)
        {
            _communityService = communityService;
            _notificationService = queueService;
        }

        #endregion Constructor

        #region Action Methods

        /// <summary>
        /// It returns the profile view
        /// </summary>
        /// <returns>It returns the profile detail</returns>
        [HttpGet]
        [Route("Profile/MyProfile/Get")]
        public async Task<JsonResult> MyProfile()
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext();
            }
            var userDetail = GetProfile(CurrentUserId);
            return new JsonResult { Data = userDetail,JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }

        
        /// <summary>
        /// Returns the partial view of Profile Entity View
        /// </summary>
        /// <param name="entityType">Content / Community</param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        [HttpGet]
        [Route("Profile/Entities/{entityType}/{currentPage}/{pageSize}")]
        public async Task<JsonResult> GetProfileEntities(EntityType entityType,  int currentPage, int pageSize)
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext();
            }
            // Initialize the page details object with current page as parameter. First time when page loads, current page is always 1.
            var pageDetails = GetPageDetails(CurrentUserId, entityType, 1);
            pageDetails.CurrentPage = currentPage;
            pageDetails.ItemsPerPage = pageSize;

            var entities = await GetEntities(CurrentUserId, entityType, pageDetails);

            SetSiteAnalyticsPrefix(HighlightType.None);

            return new JsonResult
            {
                Data = new {entities, pageInfo=pageDetails},
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }


        
        /// <summary>
        /// Save Action which saves the profile details to database.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        /// <remarks>DO NOT ADD Live Authorization for this since it will go in an indefinite loop.</remarks>
        [HttpPost]
        [Route("Profile/Save/{profileId}")]
        public JsonResult Save(long profileId, string affiliation, string aboutMe, bool isSubscribed, Guid? profileImageId, string profileName)
        {
            if (profileId == CurrentUserId)
            {
                var profileDetails = new ProfileDetails()
                {
                    ID = profileId,
                    Affiliation = Server.UrlDecode(affiliation),
                    AboutMe = aboutMe,
                    IsSubscribed = isSubscribed,
                    PictureID = profileImageId
                };

                profileName = Server.UrlDecode(profileName);
                if (!string.IsNullOrWhiteSpace(profileName))
                {
                    if (profileName.Length > 50)
                    {
                        profileDetails.FirstName = profileName.Substring(0, 50);
                        profileDetails.LastName = profileName.Substring(50);
                    }
                    else
                    {
                        profileDetails.FirstName = profileName;
                        profileDetails.LastName = string.Empty;
                    }
                }
               
                ProfileService.UpdateProfile(profileDetails);

                // This will make sure that the latest name from DB will be fetched again.
                SessionWrapper.Set<string>("CurrentUserProfileName", null);

                return new JsonResult { Data = GetProfile(CurrentUserId) };
            }

            return new JsonResult { Data = "UserId does not match current user" };
        }

        #endregion Action Methods

        #region Private Methods

        /// <summary>
        /// Gets the profile details.
        /// </summary>
        /// <param name="id">User profile ID.</param>
        /// <returns>Profile view model.</returns>
        private ProfileViewModel GetProfile(long id)
        {
            var profileDetails = ProfileService.GetProfile(id);
            var userDetail = new ProfileViewModel();
            if (profileDetails != null)
            {
                Mapper.Map(profileDetails, userDetail);
                userDetail.IsCurrentUser = CurrentUserId == profileDetails.ID;
                userDetail.ProfileName = profileDetails.GetProfileName();
                userDetail.ProfilePhotoLink = string.IsNullOrWhiteSpace(userDetail.ProfilePhotoLink) ? "~/Content/Images/profile.png" : Url.Action("Thumbnail", "File", new { id = userDetail.ProfilePhotoLink });
            }
            return userDetail;
        }

        /// <summary>
        /// Gets the page details instance.
        /// </summary>
        /// <param name="userId">User Id for whom page is rendered</param>
        /// <param name="entityType">Entity Type (Community/Content)</param>
        /// <param name="currentPage">Selected page to be rendered</param>
        /// <returns>Page details instance</returns>
        private PageDetails GetPageDetails(long userId, EntityType entityType, int currentPage)
        {
            var pageDetails = new PageDetails(currentPage);
            pageDetails.ItemsPerPage = Constants.EntitiesPerUser;

            var totalItemsForCondition = 0;
            switch (entityType)
            {
                case EntityType.Community:
                    totalItemsForCondition = ProfileService.GetCommunitiesCount(userId, userId != CurrentUserId);
                    break;
                case EntityType.Content:
                    totalItemsForCondition = ProfileService.GetContentsCount(userId, userId != CurrentUserId);
                    break;
                default:
                    break;
            }

            pageDetails.TotalPages = (totalItemsForCondition / pageDetails.ItemsPerPage) + ((totalItemsForCondition % pageDetails.ItemsPerPage == 0) ? 0 : 1);
            pageDetails.CurrentPage = currentPage > pageDetails.TotalPages ? pageDetails.TotalPages : currentPage;
            pageDetails.TotalCount = totalItemsForCondition;

            return pageDetails;
        }

        /// <summary>
        /// It returns the entity list
        /// </summary>
        /// <param name="userId">User Id for whom page is rendered</param>
        /// <param name="entityType">Entity type (Community/Content)</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>List of entity objects</returns>
        private async Task<List<EntityViewModel>> GetEntities(long userId, EntityType entityType, PageDetails pageDetails)
        {
            // TODO: Need to create a model for passing parameters to this controller
            var highlightEntities = new List<EntityViewModel>();

            if (entityType == EntityType.Community)
            {
                var communities = ProfileService.GetCommunities(userId, pageDetails, userId != CurrentUserId);
                foreach (var community in communities)
                {
                    var communityViewModel = new CommunityViewModel();
                    Mapper.Map(community, communityViewModel);
                    highlightEntities.Add(communityViewModel);
                }
            }
            else if (entityType == EntityType.Content)
            {
                var contents = ProfileService.GetContents(userId, pageDetails, userId != CurrentUserId);
                foreach (var content in contents)
                {
                    var contentViewModel = new ContentViewModel();
                    contentViewModel.SetValuesFrom(content);
                    highlightEntities.Add(contentViewModel);
                }
            }

            return highlightEntities;
        }

        /// <summary>
        /// Creates default user community.
        /// </summary>
        /// <param name="userId">User identity.</param>
        private void CreateDefaultUserCommunity(long userId)
        {
            // This will used as the default community when user is uploading a new content.
            // This community will need to have the following details:
            var communityDetails = new CommunityDetails();

            // 1. This community type should be User
            communityDetails.CommunityType = CommunityTypes.User;

            // 2. CreatedBy will be the new USER.
            communityDetails.CreatedByID = userId;

            // 3. This community is not featured.
            communityDetails.IsFeatured = false;

            // 4. Name should be NONE.
            communityDetails.Name = Resources.UserCommunityName;

            // 5. Access type should be private.
            communityDetails.AccessTypeID = (int)AccessType.Private;

            // 6. Set the category ID of general interest. We need to set the Category ID as it is a foreign key and cannot be null.
            communityDetails.CategoryID = (int)CategoryType.GeneralInterest;

            // 7. Create the community
            _communityService.CreateCommunity(communityDetails);
        }

        #endregion Private Methods
    }
}
