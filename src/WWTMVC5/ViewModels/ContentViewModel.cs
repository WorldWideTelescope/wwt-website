//-----------------------------------------------------------------------
// <copyright file="ContentViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the view for a Content with all details
    /// to be shown in Content details page.
    /// </summary>
    public class ContentViewModel : EntityBaseDetailsViewModel
    {
        /// <summary>
        /// Private member for Citation property
        /// </summary>
        private string citation;

        /// <summary>
        /// Initializes a new instance of the ContentViewModel class.
        /// </summary>
        public ContentViewModel()
        {
            Entity = EntityType.Content;
        }

        /// <summary>
        /// Gets or sets the total number of downloads of the content
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets Citation.
        /// </summary>
        public string Citation
        {
            get
            {
                return citation; 
            }
            set
            {
                citation = HttpContext.Current.Server.HtmlDecode(value);
            }
        }

        /// <summary>
        /// Gets or sets the narrator of content
        /// </summary>
        public string Narrator { get; set; }

        /// <summary>
        /// Gets or sets the associated files
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This cannot be a read only parameter")]
        public IList<AssociatedFileViewModel> AssociatedFiles { get; set; }

        /// <summary>
        /// Gets or sets the share url of social icons
        /// </summary>
        public ShareViewModel ShareUrl { get; set; }

        /// <summary>
        /// Gets or sets the content URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Need to have it as string as to be compatible with UI.")]
        public string ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is a Link or not.
        /// </summary>
        public bool IsLink { get; set; }

        /// <summary>
        /// Gets or sets file size
        /// </summary>
        public decimal Size { get; set; }

        /// <summary>
        /// Gets or sets a Video Id.
        /// </summary>
        public Guid VideoID { get; set; }

        /// <summary>
        /// Gets or sets a Video Name.
        /// </summary>
        public string VideoName { get; set; }
    }
}