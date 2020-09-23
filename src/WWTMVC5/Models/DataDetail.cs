//-----------------------------------------------------------------------
// <copyright file="DataDetail.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the model for files which are uploaded
    /// </summary>
    public class DataDetail
    {
        /// <summary>
        /// Gets or sets Content ID of the Data
        /// </summary>
        public long? ContentID { get; set; }

        /// <summary>
        /// Gets or sets name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Content MIME Type of the file.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the content Type of the Content.
        /// </summary>
        public ContentTypes ContentType { get; set; }
    }
}