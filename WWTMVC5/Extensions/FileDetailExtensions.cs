//-----------------------------------------------------------------------
// <copyright file="FileDetailExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Web;
using WWTMVC5.Models;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for AssociatedFileDetail.
    /// </summary>
    public static class FileDetailExtensions
    {
        /// <summary>
        /// Populates the FileDetail object's properties from the given Content object's properties.
        /// </summary>
        /// <param name="thisObject">Current FileDetail instance on which the extension method is called</param>
        /// <param name="details">Content model from which values to be read</param>
        public static void SetValuesFrom(this FileDetail thisObject, Content details)
        {
            if (details != null && thisObject != null)
            {
                thisObject.ContentID = details.ContentID;
                thisObject.Name = details.Filename;
                thisObject.AzureID = details.ContentAzureID;
                thisObject.AzureURL = details.ContentAzureURL;
                thisObject.Size = details.Size.HasValue ? details.Size.Value : 0;
            }
        }

        /// <summary>
        /// Populates the FileDetail object's properties from the given HttpPostedFileBase object's properties.
        /// </summary>
        /// <param name="thisObject">Current AssociatedFileDetail instance on which the extension method is called</param>
        /// <param name="details">HttpPostedFileBase model from which values to be read</param>
        public static void SetValuesFrom(this FileDetail thisObject, HttpPostedFileBase details)
        {
            if (details != null && thisObject != null)
            {
                details.InputStream.Seek(0, SeekOrigin.Begin);

                thisObject.AzureID = Guid.NewGuid();
                thisObject.MimeType = Path.GetExtension(details.FileName).GetWwtMimeType(details.ContentType);
                thisObject.DataStream = details.InputStream;
                thisObject.Name = Path.GetFileName(details.FileName);
                thisObject.Size = details.ContentLength;

                // Get Content Type of the file.
                thisObject.ContentType = Path.GetExtension(details.FileName).GetContentTypes();
            }
        }

        /// <summary>
        /// Populates the FileDetail object's properties from the given filename and fileContent object's properties.
        /// </summary>
        /// <param name="thisObject">Current FileDetail instance on which the extension method is called</param>
        /// <param name="filename">file name of the file</param>
        /// <param name="fileContent">file contents of the file</param>
        public static void SetValuesFrom(this FileDetail thisObject, string filename, Stream fileContent)
        {
            if (!string.IsNullOrWhiteSpace(filename) && fileContent != null && thisObject != null)
            {
                thisObject.ContentType = Path.GetExtension(filename).GetContentTypes();
                thisObject.AzureID = Guid.NewGuid();
                thisObject.DataStream = fileContent;
                thisObject.Name = filename;
            }
        }
    }
}