//-----------------------------------------------------------------------
// <copyright file="SearchQueryDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about Search Query.
    /// </summary>
    public class SearchQueryDetails
    {
        /// <summary>
        /// Initializes a new instance of the SearchQueryDetails class.
        /// </summary>
        public SearchQueryDetails()
        {
            this.ContentTypeFilter = new List<int>();
            this.CategoryFilter = new List<int>();
        }

        /// <summary>
        /// Gets or sets the sorting value for the page
        /// </summary>
        public SearchSortBy SortBy { get; set; }

        /// <summary>
        /// Gets the List of Content Type filters for the search page
        /// </summary>
        public IList<int> ContentTypeFilter { get; private set; }

        /// <summary>
        /// Gets the List of Category filters for the search page
        /// </summary>
        public IList<int> CategoryFilter { get; private set; }
    }
}
