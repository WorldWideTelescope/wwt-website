//-----------------------------------------------------------------------
// <copyright file="ISearchViewRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the Search View repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface ISearchViewRepository : IRepositoryBase<SearchView>
    {
        /// <summary>
        /// Gets the count of items which are satisfying the given search condition.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <param name="searchQueryDetails">Details about search like filters, sorting and pagination</param>
        /// <returns>Number of items satisfying the given search condition</returns>
        int SearchCount(string searchText, long? userId, SearchQueryDetails searchQueryDetails);

        /// <summary>
        /// Searches the communities and contents and returns the items which are satisfying the given search condition.
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">User who is searching</param>
        /// <param name="skipCount">Items to be skipped, for pagination</param>
        /// <param name="takeCount">Items to be taken, for pagination</param>
        /// <param name="searchQueryDetails">Details about search like filters, sorting and pagination</param>
        /// <returns>Items satisfying the given search condition</returns>
        IEnumerable<SearchView> Search(string searchText, long? userId, int skipCount, int takeCount, SearchQueryDetails searchQueryDetails);
    }
}