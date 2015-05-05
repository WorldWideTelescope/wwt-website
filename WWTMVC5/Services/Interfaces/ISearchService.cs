//-----------------------------------------------------------------------
// <copyright file="ISearchService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the search service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Searches the presence of the given text in communities and contents tables (Name, description and tags fields).
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <param name="pageDetails">Details about the pagination</param>
        /// <param name="searchQueryDetails">Search query details like filters, sorting and items per page</param>
        /// <returns>Communities/Contents which are having the search text</returns>
        IEnumerable<EntityViewModel> SimpleSearch(string searchText, long userId, PageDetails pageDetails, SearchQueryDetails searchQueryDetails);

        /// <summary>
        /// Searches the presence of the given text in communities and contents tables (Name, description and tags fields).
        /// </summary>
        /// <param name="searchText">Text to be searched</param>
        /// <param name="userId">Id of the user who is accessing</param>
        /// <returns>Communities/Contents which are having the search text</returns>
        IEnumerable<DeepZoomViewModel> DeepZoomSearch(string searchText, long userId);
    }
}