//-----------------------------------------------------------------------
// <copyright file="SearchViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace WWTMVC5.ViewModels
{
    public class SearchViewModel
    {
        /// <summary>
        /// Gets or sets the number of items per page selected by user in search page
        /// </summary>
        public int ResultsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the sorting value for the page
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets Content Type filters for the search page
        /// </summary>
        public string ContentTypeFilter { get; set; }

        /// <summary>
        /// Gets or sets Category filters for the search page
        /// </summary>
        public string CategoryFilter { get; set; }

        /// <summary>
        /// Gets or sets Search Results
        /// </summary>
        public IEnumerable<EntityViewModel> Results { get; set; }
    }
}