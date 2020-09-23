//-----------------------------------------------------------------------
// <copyright file="LinkDetail.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the model for files which are uploaded
    /// </summary>
    [Serializable]
    public class LinkDetail : DataDetail
    {
        /// <summary>
        /// Initializes a new instance of the LinkDetail class.
        /// </summary>
        /// <param name="contentUrl">Content URL.</param>
        /// <param name="contentID">ID of the content.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "Need to have it as string as to be compatible with UI.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = ".net framework 4.0 syntax for specifying default parameters.")]
        public LinkDetail(string contentUrl, long? contentID = null)
        {
            if (!string.IsNullOrWhiteSpace(contentUrl))
            {
                this.ContentType = ContentTypes.Link;
                this.Name = contentUrl;
                this.ContentUrl = contentUrl;
                this.MimeType = Constants.LinkMimeType;
                this.ContentID = contentID;
            }
        }

        /// <summary>
        /// Gets or sets the URL for content.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Need to have it as string as to be compatible with UI.")]
        public string ContentUrl { get; set; }
    }
}