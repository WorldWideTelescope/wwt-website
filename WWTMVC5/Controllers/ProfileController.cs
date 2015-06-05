//-----------------------------------------------------------------------
// <copyright file="ProfileController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
            this._communityService = communityService;
            _notificationService = queueService;
        }

        #endregion Constructor

        #region Action Methods

        /// <summary>
        /// It returns the profile view
        /// </summary>
        /// <returns>It returns the profile detail</returns>
        [HttpPost]
        [Route("Profile/MyProfile/Get")]
        public async Task<JsonResult> MyProfile()
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            var userDetail = GetProfile(CurrentUserId);
            return new JsonResult { Data = userDetail };
        }

        /// <summary>
        /// It returns the profile view
        /// </summary>
        /// <returns>It returns the profile detail</returns>
        [HttpGet]
        public ActionResult Index(long id)
        {
            ProfileViewModel userDetail = GetProfile(id);
            return View(userDetail);
        }

        /// <summary>
        /// Returns the partial view of Profile Entity View
        /// </summary>
        /// <param name="entityType">Content / Community</param>
        /// <param name="profileId">User profile Id</param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        [HttpPost]
        [Route("Profile/Entities/{entityType}/{currentPage}/{pageSize}")]
        public JsonResult Render(EntityType entityType,  int currentPage, int pageSize)
        {
            
            // Initialize the page details object with current page as parameter. First time when page loads, current page is always 1.
            PageDetails pageDetails = GetPageDetails(CurrentUserId, entityType, 1);
            pageDetails.CurrentPage = currentPage;
            pageDetails.ItemsPerPage = pageSize;

            List<EntityViewModel> entities = GetEntities(CurrentUserId, entityType, pageDetails);

            SetSiteAnalyticsPrefix(HighlightType.None);

            return new JsonResult {Data = new {entities=entities,pageInfo=pageDetails}};

        }

        /// <summary>
        /// Returns the partial view of Profile Entity View
        /// </summary>
        /// <param name="entityType">Content / Community</param>
        /// <param name="profileId">User profile Id</param>
        [HttpPost]
        public void AjaxRender(EntityType entityType, long profileId, int currentPage)
        {
            try
            {
                // Initialize the page details object with current page as parameter.
                PageDetails pageDetails = GetPageDetails(profileId, entityType, currentPage);

                List<EntityViewModel> highlightEntities = GetEntities(profileId, entityType, pageDetails);

                ViewData["CurrentPage"] = currentPage;
                ViewData["TotalPage"] = pageDetails.TotalPages;
                ViewData["TotalCount"] = pageDetails.TotalCount;

                // It creates the prefix for id of links
                SetSiteAnalyticsPrefix(HighlightType.None);

                PartialView("ProfileEntityView", highlightEntities).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Index Action which is default action rendering the Terms and condition window.
        /// </summary>
        /// <returns>Returns the View to be used</returns>
        /// <remarks>DO NOT ADD Live Authorization for this since it will go in an indefinite loop.</remarks>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("Profile/New/Create")]
        public async Task<JsonResult> New()
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext(_communityService, _notificationService);
            }
            LiveLoginResult result = SessionWrapper.Get<LiveLoginResult>("LiveConnectResult");
            if (result != null && result.Status == LiveConnectSessionStatus.Connected)
            {
                var profileDetails = SessionWrapper.Get<ProfileDetails>("ProfileDetails");
                
                // While creating the user, IsSubscribed to be true always.
                profileDetails.IsSubscribed = true;

                // When creating the user, by default the user type will be of regular. 
                profileDetails.UserType = UserTypes.Regular;

                profileDetails.ID = this.ProfileService.CreateProfile(profileDetails);
                SessionWrapper.Set<long>("CurrentUserID", profileDetails.ID);
                CreateDefaultUserCommunity(profileDetails.ID);

                // Send New user notification.
                _notificationService.NotifyNewEntityRequest(profileDetails, HttpContext.Request.Url.GetServerLink());
                return new JsonResult{Data = profileDetails};
            }
            return Json("error: User not logged in");
            
            
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
                ProfileDetails profileDetails = new ProfileDetails()
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
               
                this.ProfileService.UpdateProfile(profileDetails);

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
            var profileDetails = this.ProfileService.GetProfile(id);
            ProfileViewModel userDetail = new ProfileViewModel();
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
            PageDetails pageDetails = new PageDetails(currentPage);
            pageDetails.ItemsPerPage = Constants.EntitiesPerUser;

            int totalItemsForCondition = 0;
            switch (entityType)
            {
                case EntityType.Community:
                    totalItemsForCondition = this.ProfileService.GetCommunitiesCount(userId, userId != CurrentUserId);
                    break;
                case EntityType.Content:
                    totalItemsForCondition = this.ProfileService.GetContentsCount(userId, userId != CurrentUserId);
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
        private List<EntityViewModel> GetEntities(long userId, EntityType entityType, PageDetails pageDetails)
        {
            // TODO: Need to create a model for passing parameters to this controller
            List<EntityViewModel> highlightEntities = new List<EntityViewModel>();

            if (entityType == EntityType.Community)
            {
                IEnumerable<CommunityDetails> communities = this.ProfileService.GetCommunities(userId, pageDetails, userId != CurrentUserId);
                foreach (CommunityDetails community in communities)
                {
                    CommunityViewModel communityViewModel = new CommunityViewModel();
                    Mapper.Map(community, communityViewModel);
                    highlightEntities.Add(communityViewModel);
                }
            }
            else if (entityType == EntityType.Content)
            {
                IEnumerable<ContentDetails> contents = this.ProfileService.GetContents(userId, pageDetails, userId != CurrentUserId);
                foreach (ContentDetails content in contents)
                {
                    ContentViewModel contentViewModel = new ContentViewModel();
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
            CommunityDetails communityDetails = new CommunityDetails();

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
            this._communityService.CreateCommunity(communityDetails);
        }

        #endregion Private Methods
    }
}
