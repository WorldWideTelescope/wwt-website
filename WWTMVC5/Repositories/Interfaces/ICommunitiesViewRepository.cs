//-----------------------------------------------------------------------
// <copyright file="ICommunitiesViewRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the Communities view repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface ICommunitiesViewRepository : IRepositoryBase<CommunitiesView>
    {
        /// <summary>
        /// Gets the search result count for the given search text from communities
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <returns>Count of search result items</returns>
        int SearchCommunitiesCount(string searchText, long userId);

        /// <summary>
        /// Gets the search results for the given search text from communities.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <param name="skipCount">Items to be skipped based on pagination</param>
        /// <param name="takeCount">Items to be taken based on pagination</param>
        /// <returns>Search result items</returns>
        IEnumerable<CommunitiesView> SearchCommunities(string searchText, long userId, int skipCount, int takeCount);
    }
}