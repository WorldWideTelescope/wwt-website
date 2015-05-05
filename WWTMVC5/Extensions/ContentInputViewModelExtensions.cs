//-----------------------------------------------------------------------
// <copyright file="ContentInputViewModelExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using WWTMVC5.Models;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Content details.
    /// </summary>
    public static class ContentInputViewModelExtensions
    {
        /// <summary>
        /// Populates the ContentViewModel object's properties from the given ContentsView object's properties.
        /// </summary>
        /// <param name="thisObject">Current content view model on which the extension method is called</param>
        /// <param name="content">ContentInputViewModel model from which values to be read</param>
        public static void SetValuesFrom(this ContentInputViewModel thisObject, ContentDetails content)
        {
            if (thisObject != null && content != null)
            {
                thisObject.ID = content.ID;
                thisObject.Name = content.Name;
                thisObject.Description = content.Description;
                thisObject.DistributedBy = content.DistributedBy;
                thisObject.CategoryID = content.CategoryID;
                thisObject.Tags = content.Tags;
                thisObject.Citation = content.Citation;
                thisObject.AccessTypeID = content.AccessTypeID;
                thisObject.IsOffensive = content.IsOffensive;

                // Set parent properties.
                thisObject.ParentID = content.ParentID;

                thisObject.IsLink = content.ContentData.ContentType == ContentTypes.Link;

                // Set content data properties.
                if (content.ContentData is LinkDetail)
                {
                    var linkDetail = content.ContentData as LinkDetail;
                    thisObject.ContentUrl = linkDetail.ContentUrl;
                    thisObject.ContentFileDetail = string.Format(CultureInfo.InvariantCulture, "link~0~{1}~link~{0}", content.ID, Guid.Empty);
                }
                else if (content.ContentData is FileDetail)
                {
                    var fileDetail = content.ContentData as FileDetail;
                    thisObject.FileName = fileDetail.Name;
                    thisObject.ContentDataID = fileDetail.AzureID;

                    thisObject.ContentFileDetail = string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}~{1}~{2}~{3}~{4}",
                            Path.GetExtension(fileDetail.Name),
                            fileDetail.Size,
                            fileDetail.AzureID.ToString(),
                            fileDetail.MimeType,
                            content.ID);
                }

                // Set thumbnail properties.
                if (content.Thumbnail != null)
                {
                    thisObject.ThumbnailID = content.Thumbnail.AzureID;
                }

                // Set video properties.
                var videoData = content.Video as FileDetail;
                if (videoData != null)
                {
                    thisObject.VideoID = videoData.AzureID;
                    thisObject.VideoName = videoData.Name;
                }

                thisObject.TourLength = content.TourLength;

                // Set associated file details.
                UpdateAssociatedFiles(thisObject, content);
            }
        }

        /// <summary>
        /// Updates the associated files from content details model.
        /// </summary>
        /// <param name="thisObject">ContentInput ViewModel.</param>
        /// <param name="content">COntent details model.</param>
        private static void UpdateAssociatedFiles(ContentInputViewModel thisObject, ContentDetails content)
        {
            List<string> postedFileNames = new List<string>();
            List<string> postedFileDetails = new List<string>();

            if (content.AssociatedFiles != null)
            {
                foreach (var item in content.AssociatedFiles)
                {
                    string postedFileDetail = string.Empty;
                    if (item is LinkDetail)
                    {
                        postedFileDetail = string.Format(CultureInfo.InvariantCulture, "link~0~{1}~link~{0}", item.ContentID, Guid.Empty);
                    }
                    else 
                    {
                        var fileDetail = item as FileDetail;
                        if (item != null)
                        {
                            postedFileDetail = string.Format(
                                CultureInfo.InvariantCulture,
                                "{0}~{1}~{2}~{3}~{4}",
                                Path.GetExtension(fileDetail.Name),
                                fileDetail.Size,
                                fileDetail.AzureID.ToString(),
                                fileDetail.MimeType,
                                item.ContentID);
                        }
                    }

                    postedFileNames.Add(item.Name);
                    postedFileDetails.Add(postedFileDetail);
                }
            }

            thisObject.PostedFileName = postedFileNames;
            thisObject.PostedFileDetail = postedFileDetails;
        }
    }
}