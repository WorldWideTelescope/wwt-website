//-----------------------------------------------------------------------
// <copyright file="FileDetail.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the model for files which are uploaded
    /// </summary>
    [Serializable]
    public class FileDetail : DataDetail
    {        
        /// <summary>
        /// Gets or sets File Azure Id
        /// </summary>
        public Guid AzureID { get; set; }

        /// <summary>
        /// Gets or sets content azure URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Need to have it as string as to be compatible with UI.")]
        public string AzureURL { get; set; }

        /// <summary>
        /// Gets or sets file size
        /// </summary>
        public decimal Size { get; set; }

        /// <summary>
        /// Gets or sets file data stream.
        /// </summary>
        public Stream DataStream { get; set; }

        /// <summary>
        /// Gets the extension of the file.
        /// </summary>
        public string Extension
        {
            get
            {
                return Path.GetExtension(this.Name);
            }
        }

        /// <summary>
        /// Gets the filename without extension.
        /// </summary>
        public string Title
        {
            get
            {
                return Path.GetFileNameWithoutExtension(this.Name);
            }
        }
    }
}