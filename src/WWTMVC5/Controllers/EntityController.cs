//-----------------------------------------------------------------------
// <copyright file="EntityController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the Entity page request which makes request to repository and get the
    /// data about the either content or community and pushes them to the Entity View.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public class EntityController : ControllerBase
    {
        #region Members

        /// <summary>
        /// Instance of Entity Service
        /// </summary>
        private IEntityService _entityService;

        private INotificationService _notificationService;

        private ICommunityService _communityService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the EntityController class.
        /// </summary>
        /// <param name="entityService">Instance of Entity Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public EntityController(IEntityService entityService, IProfileService profileService, ICommunityService communityService,INotificationService notificationService)
            : base(profileService)
        {
            _entityService = entityService;
            _notificationService = notificationService;
            _communityService = communityService;
        }

        
        #endregion

        #region Action Methods

        /// <summary>
        /// It renders the entity list partial view
        /// </summary>
        /// <param name="highlightType">Highlight type for the entities (Featured/Latest/Popular/Related)</param>
        /// <param name="entityType">Entity type (Community/Content)</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="categoryType">Category type, default value is to get entities from all categories.</param>
        /// <param name="contentType"></param>
        /// <param name="entityId"></param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = ".net framework 4 way of passing default parameters."), HttpGet]
        [Route("Entity/Browse/{highlightType}/{entityType}/{categoryType}/{contentType}/{page}/{pageSize}")]
        
        public async Task<JsonResult> GetBrowseContent(HighlightType highlightType, EntityType entityType, int page, int pageSize, CategoryType categoryType, ContentTypes contentType, long? entityId)
        {

            var pageDetails = new PageDetails(page);
            pageDetails.ItemsPerPage = pageSize;
            var entityHighlightFilter = new EntityHighlightFilter(highlightType, categoryType, entityId, contentType);
            var highlightEntities = await GetHighlightEntities(entityType, entityHighlightFilter, pageDetails);

            

            // It creates the prefix for id of links
            SetSiteAnalyticsPrefix(highlightType);
            var result = new JsonResult
            {
                Data = new
                {
                    entities = highlightEntities,
                    pageInfo = pageDetails
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;

        }

        

        
        [HttpGet]
        [Route("Entity/Types/GetAll")]
        public async Task<JsonResult> GetAllTypes()
        {
            if (CurrentUserId == 0)
            {
                await TryAuthenticateFromHttpContext();
            }
            var highlightTypes = (from HighlightType t in Enum.GetValues(typeof (HighlightType)) select t.ToString()).ToList();
            var entityTypes = (from EntityType t in Enum.GetValues(typeof (EntityType)) select t.ToString()).ToList();
            var categoryTypes = (from CategoryType t in Enum.GetValues(typeof(CategoryType)) select t.ToString()).ToList();
            var contentTypes = (from ContentTypes t in Enum.GetValues(typeof(ContentTypes)) select t.ToString()).ToList();
            var searchTypes = (from SearchSortBy t in Enum.GetValues(typeof(SearchSortBy)) select t.ToString()).ToList();
            var admin = false;
            if (CurrentUserId != 0)
            {
                var profileDetails = SessionWrapper.Get<ProfileDetails>("ProfileDetails");
                admin = profileDetails.UserType == UserTypes.SiteAdmin;
            }
            return new JsonResult {
                Data = new {
                    highlightValues=highlightTypes,
                    entityValues=entityTypes,
                    categoryValues=categoryTypes,
                    contentValues=contentTypes,
                    searchValues=searchTypes,
                    currentUserId=CurrentUserId,
                    isAdmin=admin
                },
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }


        

        /// <summary>
        /// Controller action which gets the add thumbnail view.
        /// </summary>
        /// <param name="thumbnail">HttpPostedFileBase instance</param>
        /// <param name="entity"></param>
        [HttpPost]
        [Route("Entity/AddThumbnail/{entity}")]
        public async Task<JsonResult> AddThumbnail(HttpPostedFileBase thumbnail, EntityType entity)
        {
            try
            {
                if (CurrentUserId == 0)
                {
                    await TryAuthenticateFromHttpContext();
                }

                var thumbnailId = Guid.Empty;
                var thumnailUrl = Url.Content("~/content/images/" +
                                                 (entity == EntityType.User
                                                     ? "profile.png"
                                                     : entity == EntityType.Content
                                                         ? "defaultgenericthumbnail.png"
                                                         : "defaultcommunitythumbnail.png"));
                if (thumbnail != null)
                {
                    // Get File details.
                    var fileDetail = new FileDetail();
                    fileDetail.SetValuesFrom(thumbnail);

                    if (entity == EntityType.User)
                    {
                        // Update the size of the profile picture to 111 X 111 and upload it to temporary container in Azure.
                        fileDetail.DataStream =
                            thumbnail.InputStream.GenerateThumbnail(Constants.DefaultProfilePictureWidth,
                                Constants.DefaultProfilePictureHeight, Constants.DefaultThumbnailImageFormat);
                    }
                    else
                    {
                        // Update the size of the thumbnail to 160 X 96 and upload it to temporary container in Azure.
                        fileDetail.DataStream = thumbnail.InputStream.GenerateThumbnail(
                            Constants.DefaultThumbnailWidth, Constants.DefaultThumbnailHeight,
                            Constants.DefaultThumbnailImageFormat);
                    }

                    fileDetail.MimeType = Constants.DefaultThumbnailMimeType;

                    // Once the user publishes the content then we will move the file from temporary container to the actual container.
                    // TODO: Need to have clean up task which will delete all unused file from temporary container.
                    _entityService.UploadTemporaryFile(fileDetail);

                    thumbnailId = fileDetail.AzureID;
                    thumnailUrl = "/file/thumbnail/" + thumbnailId;
                }

                var viewModel = new DefaultThumbnailViewModel(thumbnailId, entity, string.Empty,
                    ContentTypes.Generic);
                viewModel.ThumbnailLink = thumnailUrl;

                return new JsonResult {Data = viewModel};
            }
            catch
            {
                return new JsonResult {Data = "error: user not logged in"};
            }

        }
        #endregion

        #region Private Methods

        /// <summary>
        /// It returns the entity list
        /// </summary>
        /// <param name="entityType">Entity type (Community/Content)</param>
        /// <param name="entityHighlightFilter">Entity Highlight filter for the entities (Featured/Latest/Popular/Related)</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>entity object</returns>
        private async Task<List<EntityViewModel>> GetHighlightEntities(EntityType entityType, EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            // TODO: Need to create a model for passing parameters to this controller
            var highlightEntities = new List<EntityViewModel>();

            // Set the user who is getting the highlight entities.
            entityHighlightFilter.UserID = CurrentUserId;

            // Total pages will be set by the service.
            if (pageDetails.ItemsPerPage == 0)
            {
                pageDetails.ItemsPerPage = Constants.HighlightEntitiesPerPage;
            }
            if (entityType == EntityType.Community)
            {
                var communities = await _entityService.GetCommunities(entityHighlightFilter, pageDetails);
                foreach (var community in communities)
                {
                    var communityViewModel = new CommunityViewModel();
                    Mapper.Map(community, communityViewModel);
                    highlightEntities.Add(communityViewModel);
                }
            }
            else if (entityType == EntityType.Content)
            {
                var contents = await _entityService.GetContents(entityHighlightFilter, pageDetails);
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
        /// Gets the entities for the given community.
        /// </summary>
        /// <param name="entityId">Entity Id (Community/Content)</param>
        /// <param name="entityType">Entity type (Community/Content)</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="onlyItemCount">To get only item count, not the entities. When community details page is loaded first time, 
        /// no need to get the communities, only count is enough.</param>
        /// <returns>Collection of entities</returns>
        private List<EntityViewModel> GetCommunityEntities(long entityId, EntityType entityType, PageDetails pageDetails, bool onlyItemCount)
        {
            var entities = new List<EntityViewModel>();

            // Default value is 20 if the value is not specified or wrong in the configuration file.
            // Total pages will be set by the service.
            pageDetails.ItemsPerPage = Constants.HighlightEntitiesPerPage;

            // Do not hit the database if the entity is not valid. This will happen for private communities.
            if (entityId > 0)
            {
                if (entityType == EntityType.Community || entityType == EntityType.Folder)
                {
                    var subCommunities = _entityService.GetSubCommunities(entityId, CurrentUserId, pageDetails, onlyItemCount);
                    foreach (var item in subCommunities)
                    {
                        var subCommunityViewModel = new CommunityViewModel();
                        Mapper.Map(item, subCommunityViewModel);
                        entities.Add(subCommunityViewModel);
                    }
                }
                else
                {
                    var contents = _entityService.GetContents(entityId, CurrentUserId, pageDetails);
                    foreach (var item in contents)
                    {
                        var contentViewModel = new ContentViewModel();
                        contentViewModel.SetValuesFrom(item);
                        entities.Add(contentViewModel);
                    }
                }
            }

            return entities;
        }

        #endregion
    }
}
