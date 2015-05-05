//-----------------------------------------------------------------------
// <copyright file="EntityService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the Entity Service having methods for retrieving Entity
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class EntityService : IEntityService
    {
        #region Members

        /// <summary>
        /// Instance of Contents View repository
        /// </summary>
        private IContentsViewRepository contentsViewRepository;

        /// <summary>
        /// Instance of All Contents View repository
        /// </summary>
        private IRepositoryBase<AllContentsView> allContentsViewRepository;

        /// <summary>
        /// Instance of Contents repository
        /// </summary>
        private IContentRepository contentRepository;

        /// <summary>
        /// Instance of Communities View repository
        /// </summary>
        private ICommunitiesViewRepository communitiesViewRepository;

        /// <summary>
        /// Instance of All Communities View repository
        /// </summary>
        private IRepositoryBase<AllCommunitiesView> allCommunitiesViewRepository;

        /// <summary>
        /// Instance of TopCategoryEntities repository
        /// </summary>
        private IRepositoryBase<TopCategoryEntities> topCategoryEntities;

        /// <summary>
        /// Instance of Community repository
        /// </summary>
        private ICommunityRepository communityRepository;

        /// <summary>
        /// Instance of User repository
        /// </summary>
        private IUserRepository userRepository;

        /// <summary>
        /// Instance of Blob data repository
        /// </summary>
        private IBlobDataRepository blobDataRepository;

        /// <summary>
        /// Instance of FeaturedCommunitiesView repository
        /// </summary>
        private IRepositoryBase<FeaturedCommunitiesView> featuredCommunitiesViewRepository;

        /// <summary>
        /// Instance of FeaturedCommunitiesView repository
        /// </summary>
        private IRepositoryBase<FeaturedContentsView> featuredContentsViewRepository;

        /// <summary>
        /// Instance of CommunityTags repository
        /// </summary>
        private ICommunityTagsRepository communityTagsRepository;

        /// <summary>
        /// Instance of ContentTags repository
        /// </summary>
        private IContentTagsRepository contentTagsRepository;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the EntityService class.
        /// </summary>
        /// <param name="contentsViewRepository">Instance of ContentsView repository</param>
        /// <param name="allContentsViewRepository">Instance of AllContentsView repository</param>
        /// <param name="contentRepository">Instance of Content repository</param>
        /// <param name="communitiesViewRepository">Instance of CommunitiesView repository</param>
        /// <param name="allCommunitiesViewRepository">Instance of AllCommunitiesView repository</param>
        /// <param name="topCategoryEntities">Instance of TopCategoryEntities repository</param>
        /// <param name="blobDataRepository">Instance of blob data repository</param>
        /// <param name="communityRepository">Instance of Community repository</param>
        /// <param name="userRepository">Instance of User repository</param>
        /// <param name="featuredCommunitiesViewRepository">Instance of FeaturedCommunitiesView repository</param>
        /// <param name="featuredContentsViewRepository">Instance of FeaturedContentsView repository</param>
        /// <param name="communityTagsRepository">Instance of CommunityTags repository</param>
        /// <param name="contentTagsRepository">Instance of ContentTags repository</param>
        public EntityService(
                IContentsViewRepository contentsViewRepository,
                IRepositoryBase<AllContentsView> allContentsViewRepository,
                IContentRepository contentRepository,
                ICommunitiesViewRepository communitiesViewRepository,
                IRepositoryBase<AllCommunitiesView> allCommunitiesViewRepository,
                IRepositoryBase<TopCategoryEntities> topCategoryEntities,
                IBlobDataRepository blobDataRepository,
                ICommunityRepository communityRepository,
                IUserRepository userRepository,
                IRepositoryBase<FeaturedCommunitiesView> featuredCommunitiesViewRepository,
                IRepositoryBase<FeaturedContentsView> featuredContentsViewRepository,
                ICommunityTagsRepository communityTagsRepository,
                IContentTagsRepository contentTagsRepository)
        {
            this.contentsViewRepository = contentsViewRepository;
            this.allContentsViewRepository = allContentsViewRepository;
            this.contentRepository = contentRepository;
            this.communitiesViewRepository = communitiesViewRepository;
            this.allCommunitiesViewRepository = allCommunitiesViewRepository;
            this.topCategoryEntities = topCategoryEntities;
            this.communityRepository = communityRepository;
            this.userRepository = userRepository;
            this.blobDataRepository = blobDataRepository;
            this.featuredCommunitiesViewRepository = featuredCommunitiesViewRepository;
            this.featuredContentsViewRepository = featuredContentsViewRepository;
            this.communityTagsRepository = communityTagsRepository;
            this.contentTagsRepository = contentTagsRepository;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the communities from the Layerscape database for the given highlight type and category type.
        /// Highlight can be none which gets all the communities.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>List of all communities</returns>
        public IEnumerable<CommunityDetails> GetCommunities(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { entityHighlightFilter, pageDetails });

            if (entityHighlightFilter.HighlightType == HighlightType.Featured)
            {
                return GetFeaturedCommunityDetails(entityHighlightFilter, pageDetails);
            }
            else if (entityHighlightFilter.HighlightType == HighlightType.Related)
            {
                return GetRelatedCommunityDetails(entityHighlightFilter, pageDetails);
            }
            else
            {
                return GetCommunityDetails(entityHighlightFilter, pageDetails);
            }
        }

        /// <summary>
        /// Gets all the communities from the Layerscape database including the deleted items.
        /// </summary>
        /// <param name="userID">Id of the user who is accessing</param>
        /// <param name="categoryID">Category ID for which communities to be fetched</param>
        /// <returns>List of all communities</returns>
        public IEnumerable<CommunityDetails> GetAllCommunities(long userID, int? categoryID)
        {
            Func<AllCommunitiesView, object> orderBy = c => c.LastUpdatedDatetime;
            Expression<Func<AllCommunitiesView, bool>> condition = null;
            var communityDetails = new List<CommunityDetails>();

            if (this.userRepository.IsSiteAdmin(userID))
            {
                if (categoryID != null)
                {
                    condition = (AllCommunitiesView ac) => ac.CategoryID == categoryID && ac.CommunityTypeID != (int)CommunityTypes.User;
                }
                else
                {
                    condition = (AllCommunitiesView ac) => ac.CommunityTypeID != (int)CommunityTypes.User;
                }

                foreach (var community in this.allCommunitiesViewRepository.GetItems(condition, orderBy, true))
                {
                    var communityDetail = new CommunityDetails();
                    Mapper.Map(community, communityDetail);
                    communityDetails.Add(communityDetail);
                }
            }

            return communityDetails;
        }

        /// <summary>
        /// Gets the content from the Layerscape database for the given highlight type and category type.
        /// Highlight can be none which gets all the contents.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>List of all contents</returns>
        public IEnumerable<ContentDetails> GetContents(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { entityHighlightFilter, pageDetails });

            if (entityHighlightFilter.HighlightType == HighlightType.Featured)
            {
                return GetFeaturedContentDetails(entityHighlightFilter, pageDetails);
            }
            else if (entityHighlightFilter.HighlightType == HighlightType.Related)
            {
                return GetRelatedContentDetails(entityHighlightFilter, pageDetails);
            }
            else
            {
                return GetContentDetails(entityHighlightFilter, pageDetails);
            }
        }

        /// <summary>
        /// Gets all the contents from the Layerscape database including the deleted items.
        /// </summary>
        /// <param name="userID">Id of the user who is accessing</param>
        /// <param name="categoryID">Category ID for which contents to be fetched</param>
        /// <returns>List of all Contents</returns>
        public IEnumerable<ContentDetails> GetAllContents(long userID, int? categoryID)
        {
            Func<AllContentsView, object> orderBy = c => c.LastUpdatedDatetime;
            Expression<Func<AllContentsView, bool>> condition = null;
            var contentDetails = new List<ContentDetails>();

            if (this.userRepository.IsSiteAdmin(userID))
            {
                if (categoryID != null)
                {
                    condition = (AllContentsView content) => content.CategoryID == categoryID;
                }

                foreach (var content in this.allContentsViewRepository.GetItems(condition, orderBy, true))
                {
                    var contentDetail = new ContentDetails();
                    Mapper.Map(content, contentDetail);
                    contentDetails.Add(contentDetail);
                }
            }

            return contentDetails;
        }

        /// <summary>
        /// Gets the communities from the Layerscape database for the given highlight type and category type.
        /// Highlight can be none which gets all the communities.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>List of all communities</returns>
        public IEnumerable<CommunityDetails> GetCommunities(EntityHighlightFilter entityHighlightFilter)
        {
            this.CheckNotNull(() => new { entityHighlightFilter });

            if (entityHighlightFilter.HighlightType == HighlightType.Featured)
            {
                return GetAllFeaturedCommunities(entityHighlightFilter);
            }
            else
            {
                return GetAllCommunities(entityHighlightFilter);
            }
        }

        /// <summary>
        /// Gets the content from the Layerscape database for the given highlight type and category type.
        /// Highlight can be none which gets all the contents.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>List of all contents</returns>
        public IEnumerable<ContentDetails> GetContents(EntityHighlightFilter entityHighlightFilter)
        {
            this.CheckNotNull(() => new { entityHighlightFilter });

            if (entityHighlightFilter.HighlightType == HighlightType.Featured)
            {
                return GetAllFeaturedContents(entityHighlightFilter);
            }
            else
            {
                return GetAllContents(entityHighlightFilter);
            }
        }

        /// <summary>
        /// Retrieves the sub communities of a given community. This only retrieves the immediate children.
        /// </summary>
        /// <param name="communityID">ID of the community.</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="onlyItemCount">To get only item count, not the entities. When community details page is loaded first time, 
        /// no need to get the communities, only count is enough.</param>
        /// <returns>Collection of sub communities</returns>
        public IEnumerable<CommunityDetails> GetSubCommunities(long communityID, long userId, PageDetails pageDetails, bool onlyItemCount)
        {
            this.CheckNotNull(() => new { pageDetails });

            IList<CommunityDetails> subCommunities = new List<CommunityDetails>();

            IEnumerable<long> subCommunityIDs = this.communityRepository.GetSubCommunityIDs(communityID, userId);

            // Gets the total number of direct sub communities of the community
            pageDetails.TotalCount = subCommunityIDs.Count();

            pageDetails.TotalPages = (pageDetails.TotalCount / pageDetails.ItemsPerPage) + ((pageDetails.TotalCount % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            // When community details page is loaded first time, no need to get the communities, only count is enough.
            if (!onlyItemCount && subCommunityIDs != null && subCommunityIDs.Count() > 0)
            {
                subCommunityIDs = subCommunityIDs.Skip((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage).Take(pageDetails.ItemsPerPage);
                foreach (var community in this.communityRepository.GetItems(subCommunityIDs))
                {
                    UserRole userRole = this.userRepository.GetUserRole(userId, community.CommunityID);
                    CommunityDetails communityDetails = new CommunityDetails(userRole.GetPermission());

                    // Some of the values which comes from complex objects need to be set through this method.
                    communityDetails.SetValuesFrom(community);

                    subCommunities.Add(communityDetails);
                }
            }

            return subCommunities;
        }

        /// <summary>
        /// Retrieves the contents of the given community.
        /// </summary>
        /// <param name="communityID">ID of the community.</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>An enumerable which contains the contents of the community</returns>
        public IEnumerable<ContentDetails> GetContents(long communityID, long userId, PageDetails pageDetails)
        {
            this.CheckNotNull(() => new { pageDetails });

            IList<ContentDetails> contents = new List<ContentDetails>();

            IEnumerable<long> contentIDs = this.communityRepository.GetContentIDs(communityID, userId);

            // Gets the total number of contents of the community
            pageDetails.TotalCount = contentIDs.Count();

            pageDetails.TotalPages = (pageDetails.TotalCount / pageDetails.ItemsPerPage) + ((pageDetails.TotalCount % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            if (contentIDs != null && contentIDs.Count() > 0)
            {
                contentIDs = contentIDs.Skip((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage).Take(pageDetails.ItemsPerPage);
                foreach (var content in this.contentRepository.GetItems(contentIDs))
                {
                    UserRole userRole = UserRole.Visitor;

                    if (content.CreatedByID == userId)
                    {
                        userRole = UserRole.Owner;
                    }
                    else
                    {
                        userRole = this.userRepository.GetUserRole(userId, communityID);

                        if (userRole == UserRole.Moderator)
                        {
                            // In case of user is Moderator for the parent community, he should be considered as moderator inherited so 
                            // that he will be having permissions to edit/delete this content.
                            userRole = UserRole.ModeratorInheritted;
                        }
                        else if (userRole == UserRole.Contributor)
                        {
                            // In case of user is Contributor for the parent community, he should be considered as Owner if the content
                            // is created by him. If the content is not created by him, he should be considered as Reader.
                            userRole = UserRole.Reader;
                        }
                    }

                    ContentDetails contentDetails = new ContentDetails(userRole.GetPermission());

                    // Some of the values which comes from complex objects need to be set through this method.
                    contentDetails.SetValuesFrom(content);

                    contents.Add(contentDetails);
                }
            }

            return contents;
        }

        /// <summary>
        /// Gets the top categories from the Layerscape database based on the number of contents which belongs to the category.
        /// </summary>
        /// <returns>List of top 6 categories</returns>
        public IEnumerable<EntityDetails> GetTopCategories()
        {
            IList<EntityDetails> topCategories = new List<EntityDetails>();
            var topCategoryItems = this.topCategoryEntities.GetAll((TopCategoryEntities t) => t.CategoryID);
            var contents = topCategoryItems.Where(t => t.EntityType == EntityType.Content.ToString());
            var communities = topCategoryItems.Where(t => t.EntityType == EntityType.Community.ToString());
            for (int i = 0; i < contents.Count(); i++)
            {
                var content = contents.ElementAt(i);
                ContentDetails contentDetails = new ContentDetails();
                Mapper.Map(content, contentDetails);

                if (content.ThumbnailID.HasValue)
                {
                    // TODO: Auto-mapper cannot set this nested object, need to see if there is any better approach.
                    contentDetails.Thumbnail = new FileDetail();
                    contentDetails.Thumbnail.AzureID = content.ThumbnailID.Value;
                }

                // Insert the content to the top categories collection.
                topCategories.Insert(i * 3, contentDetails);

                // Get the communities of the content which got inserted.
                var contentCommunities = communities.Where((TopCategoryEntities t) => t.CategoryID == content.CategoryID);

                // Get the first community for the current content.
                TopCategoryEntities firstCommunity = contentCommunities.ElementAtOrDefault(0);
                EntityDetails communityDetails = new EntityDetails();
                Mapper.Map(firstCommunity, communityDetails);

                if (firstCommunity != null)
                {
                    // TODO: Auto-mapper is not setting the nested Thumbnail, need to initialize and set it.
                    // Need to see if there is any better approach.
                    communityDetails.Thumbnail = new FileDetail();
                    communityDetails.Thumbnail.AzureID = firstCommunity.ThumbnailID ?? Guid.Empty;
                    topCategories.Insert(i * 3 + 1, communityDetails);
                }
                else
                {
                    // Incase if there are no communities for the current content, insert null so that the sequence is maintained.
                    topCategories.Insert(i * 3 + 1, null);
                }

                // Get the second community for the current content.
                TopCategoryEntities secondCommunity = contentCommunities.ElementAtOrDefault(1);
                communityDetails = new EntityDetails();
                Mapper.Map(secondCommunity, communityDetails);

                if (secondCommunity != null)
                {
                    // TODO: Auto-mapper is not setting the nested Thumbnail, need to initialize and set it.
                    // Need to see if there is any better approach.
                    communityDetails.Thumbnail = new FileDetail();
                    communityDetails.Thumbnail.AzureID = firstCommunity.ThumbnailID ?? Guid.Empty;
                    topCategories.Insert(i * 3 + 2, communityDetails);
                }
                else
                {
                    // Incase if there are no communities for the current content, insert null so that the sequence is maintained.
                    topCategories.Insert(i * 3 + 2, null);
                }
            }

            return topCategories.AsEnumerable<EntityDetails>();
        }

        /// <summary>
        /// Uploads the associated file to temporary container.
        /// </summary>
        /// <param name="fileDetail">Details of the associated file.</param>
        /// <returns>True if content is uploaded; otherwise false.</returns>
        public bool UploadTemporaryFile(FileDetail fileDetail)
        {
            // Make sure file detail is not null
            this.CheckNotNull(() => new { fileDetail });

            BlobDetails fileBlob = new BlobDetails()
            {
                BlobID = fileDetail.AzureID.ToString(),
                Data = fileDetail.DataStream,
                MimeType = fileDetail.MimeType
            };

            return this.blobDataRepository.UploadTemporaryFile(fileBlob);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets condition clause for communities.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>Condition clause for communities.</returns>
        private static Expression<Func<CommunitiesView, bool>> GetCommunityConditionClause(EntityHighlightFilter entityHighlightFilter)
        {
            Expression<Func<CommunitiesView, bool>> condition = null;

            // Set the order by expression for the given highlight type.
            switch (entityHighlightFilter.HighlightType)
            {
                case HighlightType.Popular:
                    condition = GetTopRatedCommunitiesCondition(entityHighlightFilter);
                    break;
            }

            string accessType = AccessType.Public.ToString();

            // Only when category type is other then "All" and also the condition is not already set (which will happen when this method is called 
            // with highlight type as Featured and category type as not "All"), condition to be set here.
            if (entityHighlightFilter.CategoryType != CategoryType.All && condition == null)
            {
                string categoryType = entityHighlightFilter.CategoryType.ToString();
                condition = (CommunitiesView c) => c.CategoryName == categoryType
                                                        && c.CommunityTypeID == (int)CommunityTypes.Community
                                                        && c.AccessType == accessType;
            }

            // Still if condition is not set, add a condition to get only public communities.
            if (condition == null)
            {
                condition = (CommunitiesView c) => c.CommunityTypeID == (int)CommunityTypes.Community && c.AccessType == accessType;
            }

            return condition;
        }

        /// <summary>
        /// Gets order by clause for communities.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>Order by clause for communities.</returns>
        private static Func<CommunitiesView, object> GetCommunityOrderByClause(EntityHighlightFilter entityHighlightFilter)
        {
            Func<CommunitiesView, object> orderBy = null;

            // Set the order by expression for the given highlight type.
            switch (entityHighlightFilter.HighlightType)
            {
                case HighlightType.Latest:
                    orderBy = (CommunitiesView c) => c.LastUpdatedDatetime;
                    break;
            }

            return orderBy;
        }

        /// <summary>
        /// Gets condition clause for contents.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>Condition clause for contents.</returns>
        private static Expression<Func<ContentsView, bool>> GetContentConditionClause(EntityHighlightFilter entityHighlightFilter)
        {
            Expression<Func<ContentsView, bool>> condition = null;

            // Set the order by expression for the given highlight type.
            switch (entityHighlightFilter.HighlightType)
            {
                case HighlightType.Popular:
                    condition = GetTopRatedContentsCondition(entityHighlightFilter);
                    break;
                case HighlightType.MostDownloaded:
                    condition = GetTopDownloadedContentsCondition(entityHighlightFilter);
                    break;
            }

            string accessType = AccessType.Public.ToString();

            // Only when category type is other then "All" and also the condition is not already set (which will happen when this method is called 
            // with highlight type as Featured and category type as not "All"), condition to be set here.
            if (entityHighlightFilter.CategoryType != CategoryType.All && condition == null)
            {
                condition = (ContentsView c) => c.CategoryID == (int)entityHighlightFilter.CategoryType && 
                    c.AccessType == accessType &&
                    entityHighlightFilter.ContentType != ContentTypes.All ? c.TypeID == (int)entityHighlightFilter.ContentType : c.TypeID > -1;
            }
            if (condition == null && entityHighlightFilter.ContentType != ContentTypes.All)
            {
                condition = (ContentsView c) => c.AccessType == accessType && c.TypeID == (int)entityHighlightFilter.ContentType;
            }
            // Still if condition is not set, add a condition to get only public contents.
            return condition ?? ((ContentsView c) => c.AccessType == accessType);
            
        }

        /// <summary>
        /// Gets order by clause for contents.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>Order by clause for contents.</returns>
        private static Func<ContentsView, object> GetContentOrderByClause(EntityHighlightFilter entityHighlightFilter)
        {
            Func<ContentsView, object> orderBy = null;

            // Set the order by expression for the given highlight type.
            switch (entityHighlightFilter.HighlightType)
            {
                case HighlightType.Latest:
                    orderBy = (ContentsView c) => c.LastUpdatedDatetime;
                    break;
                case HighlightType.MostDownloaded:
                    orderBy = (ContentsView c) => c.DownloadCount;
                    break;
            }

            return orderBy;
        }

        /// <summary>
        /// Gets condition for Featured communities.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>Condition for Featured communities.</returns>
        private static Expression<Func<FeaturedCommunitiesView, bool>> GetFeaturedCommunitiesCondition(EntityHighlightFilter entityHighlightFilter)
        {
            Expression<Func<FeaturedCommunitiesView, bool>> condition;
            string accessType = AccessType.Public.ToString();

            // In case of Featured highlight, condition to be decided based on highlight and category as well.
            condition = c =>
                c.FeaturedCategoryID == (int)entityHighlightFilter.CategoryType
                && c.CommunityTypeID == (int)CommunityTypes.Community
                && c.AccessType == accessType;

            return condition;
        }

        /// <summary>
        /// Gets condition for Related communities.
        /// </summary>
        /// <param name="relatedCommunityIds">Related community ids to be used in condition</param>
        /// <returns>Condition for Related communities.</returns>
        private static Expression<Func<CommunitiesView, bool>> GetRelatedCommunitiesCondition(IEnumerable<long> relatedCommunityIds)
        {
            Expression<Func<CommunitiesView, bool>> condition = null;

            if (relatedCommunityIds != null)
            {
                condition = (CommunitiesView c) => relatedCommunityIds.Contains(c.CommunityID);
            }

            return condition;
        }

        /// <summary>
        /// Gets condition for Top Rated communities.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>Condition for Top Rated communities.</returns>
        private static Expression<Func<CommunitiesView, bool>> GetTopRatedCommunitiesCondition(EntityHighlightFilter entityHighlightFilter)
        {
            Expression<Func<CommunitiesView, bool>> condition;

            string accessType = AccessType.Public.ToString();

            // In case of TopRated highlight, condition to be decided based on Rating and category as well.
            if (entityHighlightFilter.CategoryType == CategoryType.All)
            {
                condition = (CommunitiesView c) => c.AverageRating > 0
                    && c.RatedPeople >= Constants.MinRatedPeopleCount
                    && c.CommunityTypeID == (int)CommunityTypes.Community
                    && c.AccessType == accessType;
            }
            else
            {
                string categoryType = entityHighlightFilter.CategoryType.ToString();
                condition = (CommunitiesView c) => c.AverageRating > 0
                    && c.RatedPeople >= Constants.MinRatedPeopleCount
                    && c.CategoryName == categoryType
                    && c.CommunityTypeID == (int)CommunityTypes.Community
                    && c.AccessType == accessType;
            }

            return condition;
        }

        /// <summary>
        /// Gets condition for Featured content.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>Condition for Featured contents.</returns>
        private static Expression<Func<FeaturedContentsView, bool>> GetFeaturedContentCondition(EntityHighlightFilter entityHighlightFilter)
        {
            Expression<Func<FeaturedContentsView, bool>> condition;

            string accessType = AccessType.Public.ToString();

            // In case of Featured highlight, condition to be decided based on highlight and category as well.
            condition = c =>
                c.FeaturedCategoryID == (int)entityHighlightFilter.CategoryType
                && c.AccessType == accessType;

            return condition;
        }

        /// <summary>
        /// Gets condition for Related content.
        /// </summary>
        /// <param name="relatedContentIds">Related Content ids to be used in condition</param>
        /// <returns>Condition for Related contents.</returns>
        private static Expression<Func<ContentsView, bool>> GetRelatedContentsCondition(IEnumerable<long> relatedContentIds)
        {
            Expression<Func<ContentsView, bool>> condition = null;

            if (relatedContentIds != null)
            {
                condition = (ContentsView c) => relatedContentIds.Contains(c.ContentID);
            }

            return condition;
        }

        /// <summary>
        /// Gets condition for Top Rated Contents.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>Condition for Top Rated Content.</returns>
        private static Expression<Func<ContentsView, bool>> GetTopRatedContentsCondition(EntityHighlightFilter entityHighlightFilter)
        {
            Expression<Func<ContentsView, bool>> condition;

            string accessType = AccessType.Public.ToString();

            // In case of Top Rated highlight, condition to be decided based on rating and category as well.
            if (entityHighlightFilter.CategoryType == CategoryType.All)
            {
                if (entityHighlightFilter.ContentType != ContentTypes.All)
                {
                    condition = (ContentsView c) => c.AccessType == accessType
                                                    && c.AverageRating > 0
                                                    && c.RatedPeople >= Constants.MinRatedPeopleCount
                                                    && c.TypeID == (int) entityHighlightFilter.ContentType;
                }
                else
                {
                    condition = (ContentsView c) => c.AccessType == accessType
                                                    && c.AverageRating > 0
                                                    && c.RatedPeople >= Constants.MinRatedPeopleCount;
                }
            }
            else
            {
                if (entityHighlightFilter.ContentType != ContentTypes.All)
                {
                    condition = (ContentsView c) => c.CategoryID == (int) entityHighlightFilter.CategoryType
                                                    && c.AccessType == accessType
                                                    && c.AverageRating > 0
                                                    && c.RatedPeople >= Constants.MinRatedPeopleCount
                                                    && c.TypeID == (int)entityHighlightFilter.ContentType; 
                }
                else
                {
                    condition = (ContentsView c) => c.CategoryID == (int) entityHighlightFilter.CategoryType
                                                    && c.AccessType == accessType
                                                    && c.AverageRating > 0
                                                    && c.RatedPeople >= Constants.MinRatedPeopleCount;
                }
                
            }
            return condition;
        }

        /// <summary>
        /// Gets condition for Top Downloaded Contents.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <returns>Condition for Top Downloaded Content.</returns>
        private static Expression<Func<ContentsView, bool>> GetTopDownloadedContentsCondition(EntityHighlightFilter entityHighlightFilter)
        {
            Expression<Func<ContentsView, bool>> condition;

            string accessType = AccessType.Public.ToString();

            // In case of Top Rated highlight, condition to be decided based on rating and category as well.
            if (entityHighlightFilter.CategoryType == CategoryType.All)
            {
                if (entityHighlightFilter.ContentType != ContentTypes.All)
                {
                    condition = (ContentsView c) => c.AccessType == accessType &&
                                                    c.DownloadCount > 0 &&
                                                    c.TypeID == (int) entityHighlightFilter.ContentType;
                }
                else
                {
                    condition = (ContentsView c) => c.AccessType == accessType &&
                                                    c.DownloadCount > 0;
                }
            }
            else
            {
                if (entityHighlightFilter.ContentType != ContentTypes.All)
                {
                    condition = (ContentsView c) => c.CategoryID == (int)entityHighlightFilter.CategoryType && 
                                                    c.AccessType == accessType &&
                                                    c.DownloadCount > 0 &&
                                                    c.TypeID == (int) entityHighlightFilter.ContentType;
                }
                else
                {
                    condition = (ContentsView c) => c.CategoryID == (int) entityHighlightFilter.CategoryType &&
                                                    c.AccessType == accessType && c.DownloadCount > 0;
                }
            }

            return condition;
        }

        private IEnumerable<CommunityDetails> GetCommunityDetails(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            Func<CommunitiesView, object> orderBy = GetCommunityOrderByClause(entityHighlightFilter);
            Expression<Func<CommunitiesView, bool>> condition = GetCommunityConditionClause(entityHighlightFilter);

            // Gets the total items satisfying the
            int totalItemsForCondition = this.communitiesViewRepository.GetItemsCount(condition);

            // If TotalCount is already specified in pageDetails, need to consider that. Ignore even if there are more items in the DB.
            if (pageDetails.TotalCount > 0 && totalItemsForCondition > pageDetails.TotalCount)
            {
                totalItemsForCondition = pageDetails.TotalCount;
            }

            pageDetails.TotalPages = (totalItemsForCondition / pageDetails.ItemsPerPage) + ((totalItemsForCondition % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            IEnumerable<CommunitiesView> communities = null;

            // Only for Popular/Top Rated, there is multiple order by which needs to added here.
            if (entityHighlightFilter.HighlightType == HighlightType.Popular)
            {
                // TODO: This is a temporary fix, since multiple order by cannot be passed.
                // Need to do this in a better way, instead of getting all the items.
                communities = this.communitiesViewRepository.GetItems(condition, null, true);
                communities = communities.OrderByDescending<CommunitiesView, decimal?>((CommunitiesView c) => c.AverageRating).ThenByDescending<CommunitiesView, int?>((CommunitiesView c) => c.RatedPeople)
                                .Skip((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage)
                                .Take(pageDetails.ItemsPerPage).ToList();
            }
            else
            {
                communities = this.communitiesViewRepository.GetItems(condition, orderBy, true, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage);
            }

            var communityDetails = new List<CommunityDetails>();
            if (communities != null)
            {
                foreach (var community in communities)
                {
                    var communityDetail = new CommunityDetails();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(community, communityDetail);

                    communityDetails.Add(communityDetail);
                }
            }

            return communityDetails;
        }

        private IEnumerable<ContentDetails> GetContentDetails(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            Func<ContentsView, object> orderBy = GetContentOrderByClause(entityHighlightFilter);
            Expression<Func<ContentsView, bool>> condition = GetContentConditionClause(entityHighlightFilter);

            // Gets the total items satisfying the
            int totalItemsForCondition = this.contentsViewRepository.GetItemsCount(condition);

            // If TotalCount is already specified in pageDetails, need to consider that. Ignore even if there are more items in the DB.
            if (pageDetails.TotalCount > 0 && totalItemsForCondition > pageDetails.TotalCount)
            {
                totalItemsForCondition = pageDetails.TotalCount;
            }

            pageDetails.TotalPages = (totalItemsForCondition / pageDetails.ItemsPerPage) + ((totalItemsForCondition % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            // TODO: Passing the condition in a variable doesn't add the WHERE clause in SQL server. Need to work on this later.
            IEnumerable<ContentsView> contents = null;

            // Only for Popular/Top Rated, there is multiple order by which needs to added here.
            if (entityHighlightFilter.HighlightType == HighlightType.Popular)
            {
                // TODO: This is a temporary fix, since multiple order by cannot be passed.
                // Need to do this in a better way, instead of getting all the items.
                contents = this.contentsViewRepository.GetItems(condition, null, true);
                
                contents = contents
                    .OrderByDescending<ContentsView, decimal?>((ContentsView c) => c.AverageRating)
                    .ThenByDescending<ContentsView, int?>((ContentsView c) => c.RatedPeople)
                    .Skip((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage)
                    .Take(pageDetails.ItemsPerPage)
                    .ToList();
            }
            else
            {
                contents = this.contentsViewRepository.GetItems(condition, orderBy, true, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage);
            }

            var contentDetails = new List<ContentDetails>();
            if (contents != null)
            {
                foreach (var content in contents)
                {
                    var contentDetail = new ContentDetails();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(content, contentDetail);

                    contentDetails.Add(contentDetail);
                }
            }

            return contentDetails;
        }

        private IEnumerable<CommunityDetails> GetFeaturedCommunityDetails(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            Func<FeaturedCommunitiesView, object> orderBy = c => c.SortOrder;
            Expression<Func<FeaturedCommunitiesView, bool>> condition = GetFeaturedCommunitiesCondition(entityHighlightFilter);

            // Gets the total items satisfying the
            int totalItemsForCondition = this.featuredCommunitiesViewRepository.GetItemsCount(condition);

            // If TotalCount is already specified in pageDetails, need to consider that. Ignore even if there are more items in the DB.
            if (pageDetails.TotalCount > 0 && totalItemsForCondition > pageDetails.TotalCount)
            {
                totalItemsForCondition = pageDetails.TotalCount;
            }

            pageDetails.TotalPages = (totalItemsForCondition / pageDetails.ItemsPerPage) + ((totalItemsForCondition % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            IEnumerable<FeaturedCommunitiesView> communities = null;

            communities = this.featuredCommunitiesViewRepository.GetItems(condition, orderBy, false, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage);

            var communityDetails = new List<CommunityDetails>();
            if (communities != null)
            {
                foreach (var community in communities)
                {
                    var communityDetail = new CommunityDetails();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(community, communityDetail);

                    communityDetails.Add(communityDetail);
                }
            }

            return communityDetails;
        }

        /// <summary>
        /// Gets the Related Community details for the given community.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>Collection of related communities</returns>
        private IEnumerable<CommunityDetails> GetRelatedCommunityDetails(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            long userID = entityHighlightFilter.EntityId.HasValue ? entityHighlightFilter.EntityId.Value : 0;

            var relatedCommunityIds = this.communityTagsRepository.GetRelatedCommunityIDs(userID, entityHighlightFilter.UserID);

            Expression<Func<CommunitiesView, bool>> condition = GetRelatedCommunitiesCondition(relatedCommunityIds);

            // Gets the total items satisfying the
            int totalItemsForCondition = this.communitiesViewRepository.GetItemsCount(condition);

            // If TotalCount is already specified in pageDetails, need to consider that. Ignore even if there are more items in the DB.
            if (pageDetails.TotalCount > 0 && totalItemsForCondition > pageDetails.TotalCount)
            {
                totalItemsForCondition = pageDetails.TotalCount;
            }

            pageDetails.TotalPages = (totalItemsForCondition / pageDetails.ItemsPerPage) + ((totalItemsForCondition % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            IEnumerable<CommunitiesView> communities = null;

            relatedCommunityIds = relatedCommunityIds.Skip((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage).Take(pageDetails.ItemsPerPage);
            condition = GetRelatedCommunitiesCondition(relatedCommunityIds);

            communities = this.communitiesViewRepository.GetItems(condition, null, false);

            var communityDetails = new List<CommunityDetails>();
            if (communities != null)
            {
                foreach (var communityId in relatedCommunityIds)
                {
                    var communityDetail = new CommunityDetails();
                    CommunitiesView community = communities.Where(c => c.CommunityID == communityId).FirstOrDefault();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(community, communityDetail);

                    communityDetails.Add(communityDetail);
                }
            }

            return communityDetails;
        }

        private IEnumerable<ContentDetails> GetFeaturedContentDetails(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            Func<FeaturedContentsView, object> orderBy = c => c.SortOrder;
            Expression<Func<FeaturedContentsView, bool>> condition = GetFeaturedContentCondition(entityHighlightFilter);

            // Gets the total items satisfying the
            int totalItemsForCondition = this.featuredContentsViewRepository.GetItemsCount(condition);

            // If TotalCount is already specified in pageDetails, need to consider that. Ignore even if there are more items in the DB.
            if (pageDetails.TotalCount > 0 && totalItemsForCondition > pageDetails.TotalCount)
            {
                totalItemsForCondition = pageDetails.TotalCount;
            }

            pageDetails.TotalPages = (totalItemsForCondition / pageDetails.ItemsPerPage) + ((totalItemsForCondition % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            // TODO: Passing the condition in a variable doesn't add the WHERE clause in SQL server. Need to work on this later.
            IEnumerable<FeaturedContentsView> contents = null;

            // Only for Popular/Top Rated, there is multiple order by which needs to added here.
            contents = this.featuredContentsViewRepository.GetItems(condition, orderBy, false, (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage, pageDetails.ItemsPerPage);

            var contentDetails = new List<ContentDetails>();
            if (contents != null)
            {
                foreach (var content in contents)
                {
                    var contentDetail = new ContentDetails();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(content, contentDetail);

                    contentDetails.Add(contentDetail);
                }
            }

            return contentDetails;
        }

        /// <summary>
        /// Gets the Related Content details for the given content.
        /// </summary>
        /// <param name="entityHighlightFilter">Filters needed while retrieving collection of entities</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>Collection of related contents</returns>
        private IEnumerable<ContentDetails> GetRelatedContentDetails(EntityHighlightFilter entityHighlightFilter, PageDetails pageDetails)
        {
            long userID = entityHighlightFilter.EntityId.HasValue ? entityHighlightFilter.EntityId.Value : 0;

            var relatedContentIds = this.contentTagsRepository.GetRelatedContentIDs(userID, entityHighlightFilter.UserID);

            Expression<Func<ContentsView, bool>> condition = GetRelatedContentsCondition(relatedContentIds);

            // Gets the total items satisfying the
            int totalItemsForCondition = this.contentsViewRepository.GetItemsCount(condition);

            // If TotalCount is already specified in pageDetails, need to consider that. Ignore even if there are more items in the DB.
            if (pageDetails.TotalCount > 0 && totalItemsForCondition > pageDetails.TotalCount)
            {
                totalItemsForCondition = pageDetails.TotalCount;
            }

            pageDetails.TotalPages = (totalItemsForCondition / pageDetails.ItemsPerPage) + ((totalItemsForCondition % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            IEnumerable<ContentsView> contents = null;

            relatedContentIds = relatedContentIds.Skip((pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage).Take(pageDetails.ItemsPerPage);
            condition = GetRelatedContentsCondition(relatedContentIds);

            contents = this.contentsViewRepository.GetItems(condition, null, false);

            var contentDetails = new List<ContentDetails>();
            if (contents != null)
            {
                foreach (var contentID in relatedContentIds)
                {
                    var contentDetail = new ContentDetails();
                    ContentsView content = contents.Where(c => c.ContentID == contentID).FirstOrDefault();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(content, contentDetail);

                    contentDetails.Add(contentDetail);
                }
            }

            return contentDetails;
        }

        private IEnumerable<CommunityDetails> GetAllFeaturedCommunities(EntityHighlightFilter entityHighlightFilter)
        {
            Func<FeaturedCommunitiesView, object> orderBy = c => c.SortOrder;

            Expression<Func<FeaturedCommunitiesView, bool>> condition = condition = c => c.FeaturedCategoryID == (int)entityHighlightFilter.CategoryType
                    && (c.CommunityTypeID == (int)CommunityTypes.Community || c.CommunityTypeID == (int)CommunityTypes.Folder);

            IEnumerable<FeaturedCommunitiesView> communities = this.featuredCommunitiesViewRepository.GetItems(condition, orderBy, false);

            var communityDetails = new List<CommunityDetails>();
            if (communities != null)
            {
                foreach (var community in communities)
                {
                    var communityDetail = new CommunityDetails();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(community, communityDetail);

                    communityDetails.Add(communityDetail);
                }
            }

            return communityDetails;
        }

        private IEnumerable<ContentDetails> GetAllFeaturedContents(EntityHighlightFilter entityHighlightFilter)
        {
            Func<FeaturedContentsView, object> orderBy = c => c.SortOrder;
            Expression<Func<FeaturedContentsView, bool>> condition = condition = c => c.FeaturedCategoryID == (int)entityHighlightFilter.CategoryType;

            IEnumerable<FeaturedContentsView> contents = this.featuredContentsViewRepository.GetItems(condition, orderBy, false);

            var contentDetails = new List<ContentDetails>();
            if (contents != null)
            {
                foreach (var content in contents)
                {
                    var contentDetail = new ContentDetails();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(content, contentDetail);

                    contentDetails.Add(contentDetail);
                }
            }

            return contentDetails;
        }

        private IEnumerable<CommunityDetails> GetAllCommunities(EntityHighlightFilter entityHighlightFilter)
        {
            Func<CommunitiesView, object> orderBy = c => c.LastUpdatedDatetime;

            string accessType = AccessType.Public.ToString();
            Expression<Func<CommunitiesView, bool>> condition = null;
            if (entityHighlightFilter.CategoryType != CategoryType.All)
            {
                condition = c => c.CategoryID == (int)entityHighlightFilter.CategoryType && c.AccessType == accessType
                    && (c.CommunityTypeID == (int)CommunityTypes.Community || c.CommunityTypeID == (int)CommunityTypes.Folder);
            }
            else
            {
                condition = c => c.AccessType == accessType && 
                    (c.CommunityTypeID == (int)CommunityTypes.Community || c.CommunityTypeID == (int)CommunityTypes.Folder);
            }

            IEnumerable<CommunitiesView> communities = this.communitiesViewRepository.GetItems(condition, orderBy, true);

            var communityDetails = new List<CommunityDetails>();
            if (communities != null)
            {
                foreach (var community in communities)
                {
                    var communityDetail = new CommunityDetails();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(community, communityDetail);

                    communityDetails.Add(communityDetail);
                }
            }

            return communityDetails;
        }

        private IEnumerable<ContentDetails> GetAllContents(EntityHighlightFilter entityHighlightFilter)
        {
            Func<ContentsView, object> orderBy = c => c.LastUpdatedDatetime;

            string accessType = AccessType.Public.ToString();
            Expression<Func<ContentsView, bool>> condition = c => c.AccessType == accessType;
            if (entityHighlightFilter.CategoryType != CategoryType.All)
            {
                condition = c => c.CategoryID == (int)entityHighlightFilter.CategoryType && c.AccessType == accessType;
            }

            IEnumerable<ContentsView> contents = this.contentsViewRepository.GetItems(condition, orderBy, true);

            var contentDetails = new List<ContentDetails>();
            if (contents != null)
            {
                foreach (var content in contents)
                {
                    var contentDetail = new ContentDetails();

                    // Some of the values which comes from complex objects need to be set through this method.
                    Mapper.Map(content, contentDetail);

                    contentDetails.Add(contentDetail);
                }
            }

            return contentDetails;
        }

        #endregion
    }
}
