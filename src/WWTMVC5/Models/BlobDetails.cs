//-----------------------------------------------------------------------
// <copyright file="BlobDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Details of the blob.
    /// </summary>
    public class BlobDetails
    {
        /// <summary>
        /// Gets or sets the blob id.
        /// </summary>
        public string BlobID { get; set; }

        /// <summary>
        /// Gets or sets the content MIME type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the data the blob as a stream.
        /// </summary>
        public Stream Data { get; set; }
    }
}