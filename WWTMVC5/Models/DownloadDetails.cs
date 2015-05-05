//-----------------------------------------------------------------------
// <copyright file="DownloadDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    [Serializable]
    public class DownloadDetails
    {
        /// <summary>
        /// Gets or sets filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets file size
        /// </summary>
        public decimal Size { get; set; }

        /// <summary>
        /// Gets or sets content azure id.
        /// </summary>
        public Guid ContentAzureID { get; set; }

        /// <summary>
        /// Gets or sets content azure URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Need to have it as string as to be compatible with UI.")]
        public string ContentAzureURL { get; set; }
    }
}