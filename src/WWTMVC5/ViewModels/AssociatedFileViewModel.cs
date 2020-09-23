//-----------------------------------------------------------------------
// <copyright file="AssociatedFileViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the model for rendering the associated files
    /// to be shown in content page.
    /// </summary>
    public class AssociatedFileViewModel
    {
        /// <summary>
        /// Gets or sets Content ID of the Data
        /// </summary>
        public long? ContentID { get; set; }

        /// <summary>
        /// Gets or sets File Id
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets file name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets file thumbnail
        /// </summary>
        public Uri Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets file size
        /// </summary>
        public decimal Size { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is a Link or not.
        /// </summary>
        public bool IsLink { get; set; }
    }
}