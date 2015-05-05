//-----------------------------------------------------------------------
// <copyright file="ContentDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the details about a Content.
    /// </summary>
    [Serializable]
    public class ContentDetails : EntityDetails
    {
        /// <summary>
        /// Initializes a new instance of the ContentDetails class.
        /// </summary>
        public ContentDetails()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ContentDetails class.
        /// </summary>
        /// <param name="permission">Permissions of the user on the content</param>
        public ContentDetails(Permission permission)
            : base(permission)
        {
        }

        /// <summary>
        /// Gets or sets Video details.
        /// </summary>
        public DataDetail Video { get; set; }

        /// <summary>
        /// Gets or sets Video details.
        /// </summary>
        public DataDetail ContentData { get; set; }

        /// <summary>
        /// Gets or sets the tour length value.
        /// </summary>
        public string TourLength { get; set; }

        /// <summary>
        /// Gets or sets download count.
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets Citation.
        /// </summary>
        public string Citation { get; set; }

        /// <summary>
        /// Gets or sets the associated files.
        /// </summary>
        public IEnumerable<DataDetail> AssociatedFiles { get; set; }
    }
}
