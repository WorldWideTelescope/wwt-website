//-----------------------------------------------------------------------
// <copyright file="SearchService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Services
{
    /// <summary>
    /// Class representing the Search service having methods for searching the community and content
    /// details from SQL Azure Layerscape database.
    /// </summary>
    public class SearchService : ISearchService
    {
        /// <summary>
        /// Instance of CommunitiesView repository
        /// </summary>
        private ICommunitiesViewRepository _communitiesViewRepository;

        /// <summary>
        /// Instance of ContentsView repository
        /// </summary>
        private IContentsViewRepository _contentsViewRepository;

        /// <summary>
        /// Instance of SearchView repository
        /// </summary>
        private ISearchViewRepository _searchViewRepository;

        /// <summary>
        /// Instance of User repository
        /// </summary>
        private IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the SearchService class.
        /// </summary>
        /// <param name="communitiesViewRepository">Instance of CommunitiesView repository</param>
        /// <param name="contentsViewRepository">Instance of ContentsView repository</param>
        /// <param name="searchViewRepository">Instance of SearchView repository</param>
        /// <param name="userRepository">Instance of User repository</param>
        public SearchService(
                ICommunitiesViewRepository communitiesViewRepository,
                IContentsViewRepository contentsViewRepository,
                ISearchViewRepository searchViewRepository,
                IUserRepository userRepository)
        {
            this._communitiesViewRepository = communitiesViewRepository;
            this._contentsViewRepository = contentsViewRepository;
            this._searchViewRepository = searchViewRepository;
            this._userRepository = userRepository;
        }

        /// <summary>
        /// Searches the presence of the given text in communities and contents tables (Name, description and tags fields).
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <returns>Communities/Contents which are having the search text</returns>
        public async Task<IEnumerable<EntityViewModel>> SimpleSearch(string searchText, long userId, PageDetails pageDetails, SearchQueryDetails searchQueryDetails)
        {
            // Make sure pageDetails is not null
            this.CheckNotNull(() => new { pageDetails });

            IList<EntityViewModel> searchResults = new List<EntityViewModel>();

            // User Id to be used while searching. This will be used to see whether user is having permission or not.
            long? searchUserId = userId;

            if (_userRepository.GetUserRole(userId, null) == UserRole.SiteAdmin)
            {
                // Set user id as 0 for Site Administrators, so that role check will be ignored for private communities
                searchUserId = null;
            }

            // Gets the total communities/contents satisfying the search condition
            pageDetails.TotalCount = _searchViewRepository.SearchCount(searchText, searchUserId, searchQueryDetails);

            // Set the total pages for the search term
            pageDetails.TotalPages = (pageDetails.TotalCount / pageDetails.ItemsPerPage) + ((pageDetails.TotalCount % pageDetails.ItemsPerPage == 0) ? 0 : 1);

            // Get the skip count and take count for the given page
            var skipCount = (pageDetails.CurrentPage - 1) * pageDetails.ItemsPerPage;
            var takeCount = pageDetails.ItemsPerPage;
            var results = await _searchViewRepository.SearchAsync(searchText, searchUserId, skipCount, takeCount, searchQueryDetails);
            foreach (var entity in results)
            {
                EntityViewModel entityViewModel;

                if (entity.Entity == EntityType.Content.ToString())
                {
                    // In case of Content, need to create ContentViewModel instance which is expected by SearchResultView.
                    entityViewModel = new ContentViewModel();

                    // This is needed to avoid the FxCop warning.
                    var contentViewModel = entityViewModel as ContentViewModel;

                    // Setting the properties which are specific to Contents.
                    contentViewModel.IsLink = entity.ContentType == (int)ContentTypes.Link;
                    contentViewModel.ContentUrl = entity.ContentType == (int)ContentTypes.Link ? entity.ContentUrl : string.Empty;
                    contentViewModel.ContentAzureID = entity.ContentType == (int)ContentTypes.Link ? Guid.Empty : entity.ContentAzureID;
                }
                else
                {
                    entityViewModel = new EntityViewModel();
                }

                Mapper.Map(entity, entityViewModel);
                searchResults.Add(entityViewModel);
            }

            // TODO: Need to send the results based on relevance with following order: Title, Description, Tags and Parent.
            return searchResults;
        }

        /// <summary>
        /// Searches the presence of the given text in communities and contents tables (Name, description and tags fields).
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>Communities/Contents which are having the search text</returns>
        public Task<IEnumerable<DeepZoomViewModel>> DeepZoomSearch(string searchText, long userId)
        {
            var searchResults = new List<DeepZoomViewModel>();

            var communitiesResult = _communitiesViewRepository.SearchCommunities(searchText, userId, 0, Constants.PivotResultsCount * 2);
            var contentResult = _contentsViewRepository.SearchContents(searchText, userId, 0, Constants.PivotResultsCount * 2);
            var communitiesResultCount = communitiesResult.Count();
            var contentResultCount = contentResult.Count();
            var communityTakeCount = Constants.PivotResultsCount;
            var contentTakeCount = Constants.PivotResultsCount;

            //// If communities count is less than PivotResultsCount and content is more than PivotResultsCount
            if (communitiesResultCount < Constants.PivotResultsCount && contentResultCount > Constants.PivotResultsCount)
            {
                contentTakeCount = (Constants.PivotResultsCount * 2) - communitiesResultCount;
            }
            else if (contentResultCount < Constants.PivotResultsCount && communitiesResultCount > Constants.PivotResultsCount)
            {
                communityTakeCount = (Constants.PivotResultsCount * 2) - contentResultCount;
            }

            foreach (CommunitiesView community in communitiesResult.ToList().Take(communityTakeCount))
            {
                var communityViewModel = new DeepZoomViewModel();
                communityViewModel.SetValuesFrom(community);
                searchResults.Add(communityViewModel);
            }

            // Searching the contents for the given search text
            foreach (var content in contentResult.ToList().Take(contentTakeCount))
            {
                var contentViewModel = new DeepZoomViewModel();
                contentViewModel.SetValuesFrom(content);
                searchResults.Add(contentViewModel);
            }

            // TODO: Need to send the results based on relevance with following order: Title, Description, Tags and Parent.
            return Task.FromResult(searchResults.OrderByDescending(item => item.Rating).AsEnumerable());
        }
    }
}
