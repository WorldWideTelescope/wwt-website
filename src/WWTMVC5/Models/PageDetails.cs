//-----------------------------------------------------------------------
// <copyright file="PageDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about the page when retrieving collection of entities.
    /// </summary>
    [Serializable]
    public class PageDetails
    {
        /// <summary>
        /// Initializes a new instance of the PageDetails class.
        /// </summary>
        /// <param name="currentPage">Current page to be fetched</param>
        public PageDetails(int currentPage)
        {
            this.CurrentPage = currentPage;
        }

        /// <summary>
        /// Gets or sets the number of items per page
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the current page to be fetched
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total number of entities.
        /// </summary>
        public int TotalCount { get; set; }
    }
}