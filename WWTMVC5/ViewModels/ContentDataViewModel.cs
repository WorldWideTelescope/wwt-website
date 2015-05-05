//-----------------------------------------------------------------------
// <copyright file="ContentDataViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the view for a Content Data with all details
    /// to be shown in Add Content section of publish content page.
    /// </summary>
    public class ContentDataViewModel
    {
        /// <summary>
        /// Gets or sets the ID of the content
        /// </summary>
        public long ContentID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the content data
        /// </summary>
        public Guid ContentDataID { get; set; }

        /// <summary>
        /// Gets or sets file name of the content.
        /// </summary>
        public string ContentFileName { get; set; }

        /// <summary>
        /// Gets or sets the file details about the content data
        /// </summary>
        public string ContentFileDetail { get; set; }

        /// <summary>
        /// Gets or sets the title of the tour
        /// </summary>
        public string TourTitle { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail of the tour
        /// </summary>
        public string TourThumbnail { get; set; }

        /// <summary>
        /// Gets or sets the description of the tour
        /// </summary>
        public string TourDescription { get; set; }

        /// <summary>
        /// Gets or sets the tour distributed by value.
        /// </summary>
        public string TourDistributedBy { get; set; }

        /// <summary>
        /// Gets or sets the tour length value.
        /// </summary>
        public string TourLength { get; set; }

        /// <summary>
        /// Gets or sets the tour thumbnail link.
        /// </summary>
        public string ThumbnailLink { get; set; }
    }
}