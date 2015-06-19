//-----------------------------------------------------------------------
// <copyright file="IContentsViewRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the Contents view repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IContentsViewRepository : IRepositoryBase<ContentsView>
    {
        /// <summary>
        /// Retrieves the multiple instances of contents for the given IDs.
        /// </summary>
        /// <param name="contentIDs">
        /// Content IDs.
        /// </param>
        /// <param name="communityId">Community Id</param>
        /// <returns>
        /// Collection of ContentsView.
        /// </returns>
        IEnumerable<ContentsView> GetItems(IEnumerable<long> contentIDs, long communityId);

        /// <summary>
        /// Gets the search result count for the given search text from contents.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <returns>Count of search result items</returns>
        Task<int> SearchContentsCount(string searchText, long userId);

        /// <summary>
        /// Gets the search results for the given search text from contents.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <param name="skipCount">Items to be skipped based on pagination</param>
        /// <param name="takeCount">Items to be taken based on pagination</param>
        /// <returns>Search result items</returns>
        IEnumerable<ContentsView> SearchContents(string searchText, long userId, int skipCount, int takeCount);

        /// <summary>
        /// Gets total consumed size for the user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>Total consumed size.</returns>
        decimal GetConsumedSize(long userId);
    }
}