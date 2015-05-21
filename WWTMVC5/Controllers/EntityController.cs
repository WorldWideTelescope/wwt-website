//-----------------------------------------------------------------------
// <copyright file="EntityController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        private IEntityService entityService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the EntityController class.
        /// </summary>
        /// <param name="entityService">Instance of Entity Service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public EntityController(IEntityService entityService, IProfileService profileService)
            : base(profileService)
        {
            this.entityService = entityService;
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
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = ".net framework 4 way of passing default parameters."), HttpPost]
        [Route("Entity/RenderJson/{highlightType}/{entityType}/{categoryType}/{contentType}/{page}/{pageSize}")]
        public JsonResult RenderJson(HighlightType highlightType, EntityType entityType, int page, int pageSize, CategoryType categoryType, ContentTypes contentType, long? entityId)
        {

            PageDetails pageDetails = new PageDetails(page);
            pageDetails.ItemsPerPage = pageSize;
            EntityHighlightFilter entityHighlightFilter = new EntityHighlightFilter(highlightType, categoryType, entityId, contentType);
            List<EntityViewModel> highlightEntities = GetHighlightEntities(entityType, entityHighlightFilter, pageDetails);

            

            // It creates the prefix for id of links
            SetSiteAnalyticsPrefix(highlightType);
            var result = new JsonResult
            {
                Data = new
                {
                    entities = highlightEntities,
                    pageInfo = pageDetails
                }
            };
            return result;

        }

        /// <summary>
        /// returns the entity detail data
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityType"></param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = ".net framework 4 way of passing default parameters."), HttpPost]
        [Route("Entity/RenderDetailJson/{entityId}/{entityType=Content}")]
        public JsonResult RenderDetailJson(long entityId, EntityType entityType)
        {

            PageDetails pageDetails = new PageDetails(1) {ItemsPerPage = 1};
            EntityHighlightFilter entityHighlightFilter = new EntityHighlightFilter(HighlightType.None, CategoryType.All, entityId);
            List<EntityViewModel> entityResult = GetHighlightEntities(entityType, entityHighlightFilter, pageDetails);
            
            SetSiteAnalyticsPrefix(HighlightType.None);
            var result = new JsonResult
            {
                Data = new
                {
                    entity = entityResult
                }
            };
            return result;

        }

        
        [HttpPost]
        [Route("Entity/Types/GetAll")]
        public JsonResult GetAllTypes()
        {
            var highlightTypes = (from HighlightType t in Enum.GetValues(typeof (HighlightType)) select t.ToString()).ToList();
            var entityTypes = (from EntityType t in Enum.GetValues(typeof (EntityType)) select t.ToString()).ToList();
            var categoryTypes = (from CategoryType t in Enum.GetValues(typeof(CategoryType)) select t.ToString()).ToList();
            var contentTypes = (from ContentTypes t in Enum.GetValues(typeof(ContentTypes)) select t.ToString()).ToList();
            var searchTypes = (from SearchSortBy t in Enum.GetValues(typeof(SearchSortBy)) select t.ToString()).ToList();
            var admin = false;
            if (CurrentUserID != 0)
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
                    currentUserId=this.CurrentUserID,
                    isAdmin=admin
                }
            };
        }


        /// <summary>
        /// It renders the entity list partial view
        /// </summary>
        /// <param name="highlightType">Highlight type for the entities (Featured/Latest/Popular/Related)</param>
        /// <param name="entityType">Entity type (Community/Content)</param>
        /// <param name="entityId">Entity Id (Community/Content)</param>
        /// <param name="currentPage">Selected page</param>
        /// <param name="categoryType">Category type, default value is to get entities from all categories.</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = ".net framework 4 way of passing default parameters."), HttpPost]
        public void AjaxRender(HighlightType highlightType, EntityType entityType, long? entityId, int currentPage, CategoryType categoryType = CategoryType.All)
        {
            try
            {
                // Initialize the page details object with current page as parameter.
                PageDetails pageDetails = new PageDetails(currentPage);
                EntityHighlightFilter entityHighlightFilter = new EntityHighlightFilter(highlightType, categoryType, entityId);
                List<EntityViewModel> highlightEntities = GetHighlightEntities(entityType, entityHighlightFilter, pageDetails);
                ViewData["CurrentPage"] = currentPage;
                ViewData["TotalPage"] = pageDetails.TotalPages;

                // It creates the prefix for id of links
                SetSiteAnalyticsPrefix(highlightType);

                PartialView("EntityView", highlightEntities).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// It renders the entity list partial view
        /// </summary>
        /// <param name="entityId">Entity Id (Community/Content)</param>
        /// <param name="entityType">Entity type (Community/Content)</param>
        [ChildActionOnly]
        public void RenderList(long entityId, EntityType entityType)
        {
            try
            {
                // Initialize the page details object with current page as parameter. First time when page loads, current page is always 1.
                PageDetails pageDetails = new PageDetails(1);
                List<EntityViewModel> entities = GetCommunityEntities(entityId, entityType, pageDetails, true);

                // TODO: Model should be used here instead of so many parameters
                ViewData["CurrentPage"] = 1;
                ViewData["TotalPage"] = pageDetails.TotalPages;
                ViewData["TotalCount"] = pageDetails.TotalCount;

                // It creates the prefix for id of links
                SetSiteAnalyticsPrefix(HighlightType.None);

                PartialView("EntityView", entities).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// It renders the entity list partial view
        /// </summary>
        /// <param name="entityId">Entity Id (Community/Content)</param>
        /// <param name="entityType">Entity type (Community/Content)</param>
        /// <param name="currentPage">Selected page</param>
        [HttpPost]
        public void AjaxRenderList(long entityId, EntityType entityType, int currentPage)
        {
            try
            {
                // Initialize the page details object with current page as parameter.
                PageDetails pageDetails = new PageDetails(currentPage);
                List<EntityViewModel> entities = GetCommunityEntities(entityId, entityType, pageDetails, false);

                // TODO: Model should be used here instead of so many parameters
                ViewData["CurrentPage"] = pageDetails.CurrentPage;
                ViewData["TotalPage"] = pageDetails.TotalPages;
                ViewData["TotalCount"] = pageDetails.TotalCount;

                // It creates the prefix for id of links
                SetSiteAnalyticsPrefix(HighlightType.None);

                PartialView("EntityView", entities).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Action for handling the top categories request.
        /// </summary>
        [ChildActionOnly]
        [OutputCache(Duration = 3600)]
        public void Top()
        {
            try
            {
                IEnumerable<EntityDetails> topCategories = this.entityService.GetTopCategories();
                List<TopCategoryViewModel> topCategoryViewModel = new List<TopCategoryViewModel>();

                for (int i = 0; i < topCategories.Count() / 3; i++)
                {
                    TopCategoryViewModel topCategory = new TopCategoryViewModel();

                    ContentViewModel content = new ContentViewModel();
                    topCategory.Content = Mapper.Map((ContentDetails)topCategories.ElementAtOrDefault(i * 3), content);
                    if (topCategories.ElementAtOrDefault(i * 3) != null)
                    {
                        topCategory.Content.ThumbnailID = topCategories.ElementAtOrDefault(i * 3).Thumbnail.AzureID;
                    }
                    topCategory.Category = (topCategory.Content != null) ? topCategory.Content.Category : CategoryType.All;

                    EntityViewModel firstCommunity = new EntityViewModel();
                    firstCommunity = Mapper.Map(topCategories.ElementAtOrDefault(i * 3 + 1), firstCommunity);
                    if (firstCommunity != null)
                    {
                        topCategory.Category = firstCommunity.Category;
                        topCategory.Communities.Add(firstCommunity);
                    }

                    EntityViewModel secondCommunity = new EntityViewModel();
                    secondCommunity = Mapper.Map(topCategories.ElementAtOrDefault(i * 3 + 2), secondCommunity);
                    if (secondCommunity != null)
                    {
                        topCategory.Category = secondCommunity.Category;
                        topCategory.Communities.Add(secondCommunity);
                    }

                    // If there are no contents and communities available, do not add it to the top category view model.
                    if (topCategory.Category != CategoryType.All)
                    {
                        topCategoryViewModel.Add(topCategory);
                    }
                }

                // It creates the prefix for id of links
                SetSiteAnalyticsPrefix(HighlightType.None);

                PartialView("TopCategoryView", topCategoryViewModel).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Controller action which gets the add thumbnail partial view.
        /// </summary>
        [HttpGet]
        
        public void AddThumbnail(EntityType entityType)
        {
            try
            {
                DefaultThumbnailViewModel viewModel = new DefaultThumbnailViewModel(Guid.Empty, entityType, string.Empty, ContentTypes.Generic);

                PartialView("AddThumbnailView", viewModel).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Controller action which gets the add thumbnail view.
        /// </summary>
        /// <param name="thumbnail">HttpPostedFileBase instance</param>
        /// <param name="entity"></param>
        [HttpPost]
        //
        //[ValidateAntiForgeryToken]
        [Route("Entity/AddThumbnail/{entity}")]
        public JsonResult AddThumbnail(HttpPostedFileBase thumbnail, EntityType entity)
        {
            try
            {
                this.CheckNotNull(() => new {CurrentUserID});

                Guid thumbnailID = Guid.Empty;
                string thumnailURL = Url.Content("~/content/images/" +
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
                    this.entityService.UploadTemporaryFile(fileDetail);

                    thumbnailID = fileDetail.AzureID;
                    thumnailURL = "/file/thumbnail/" + thumbnailID;
                }

                DefaultThumbnailViewModel viewModel = new DefaultThumbnailViewModel(thumbnailID, entity, string.Empty,
                    ContentTypes.Generic);
                viewModel.ThumbnailLink = thumnailURL;

                return new JsonResult {Data = viewModel};
            }
            catch
            {
                return new JsonResult {Data = "User not logged in"};
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
        private List<EntityViewModel> GetHighlightEntities(EntityType entityType, EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            // TODO: Need to create a model for passing parameters to this controller
            List<EntityViewModel> highlightEntities = new List<EntityViewModel>();

            // Set the user who is getting the highlight entities.
            entityHighlightFilter.UserID = this.CurrentUserID;

            // Total pages will be set by the service.
            if (pageDetails.ItemsPerPage == 0)
            {
                pageDetails.ItemsPerPage = Constants.HighlightEntitiesPerPage;
            }
            if (entityType == EntityType.Community)
            {
                IEnumerable<CommunityDetails> communities = this.entityService.GetCommunities(entityHighlightFilter, pageDetails);
                foreach (var community in communities)
                {
                    CommunityViewModel communityViewModel = new CommunityViewModel();
                    Mapper.Map(community, communityViewModel);
                    highlightEntities.Add(communityViewModel);
                }
            }
            else if (entityType == EntityType.Content)
            {
                IEnumerable<ContentDetails> contents = this.entityService.GetContents(entityHighlightFilter, pageDetails);
                foreach (var content in contents)
                {
                    ContentViewModel contentViewModel = new ContentViewModel();
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
            List<EntityViewModel> entities = new List<EntityViewModel>();

            // Default value is 20 if the value is not specified or wrong in the configuration file.
            // Total pages will be set by the service.
            pageDetails.ItemsPerPage = Constants.HighlightEntitiesPerPage;

            // Do not hit the database if the entity is not valid. This will happen for private communities.
            if (entityId > 0)
            {
                if (entityType == EntityType.Community || entityType == EntityType.Folder)
                {
                    var subCommunities = this.entityService.GetSubCommunities(entityId, this.CurrentUserID, pageDetails, onlyItemCount);
                    foreach (var item in subCommunities)
                    {
                        CommunityViewModel subCommunityViewModel = new CommunityViewModel();
                        Mapper.Map(item, subCommunityViewModel);
                        entities.Add(subCommunityViewModel);
                    }
                }
                else
                {
                    var contents = this.entityService.GetContents(entityId, this.CurrentUserID, pageDetails);
                    foreach (var item in contents)
                    {
                        ContentViewModel contentViewModel = new ContentViewModel();
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
