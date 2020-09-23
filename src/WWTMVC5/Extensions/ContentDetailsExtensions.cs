//-----------------------------------------------------------------------
// <copyright file="ContentDetailsExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WWTMVC5.Models;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Content details.
    /// </summary>
    public static class ContentDetailsExtensions
    {
        /// <summary>
        /// Populates the ContentDetails object's properties from the given Content's object's properties.
        /// </summary>
        /// <param name="thisObject">Current content details model on which the extension method is called</param>
        /// <param name="content">ContentsView model from which values to be read</param>
        public static void SetValuesFrom(this ContentDetails thisObject, Content content)
        {
            if (thisObject != null && content != null)
            {
                var start = DateTime.Now;
                // Populate the base values using the EntityViewModel's SetValuesFrom method.
                (thisObject as EntityDetails).SetValuesFrom(content);
                
                var contentData = new DataDetail();
                thisObject.ContentData = contentData.SetValuesFrom(content);
                
                thisObject.Citation = content.Citation;
                thisObject.DownloadCount = content.DownloadCount ?? 0;

                // Get the distributed by user.
                thisObject.DistributedBy = content.DistributedBy;

                // Produced by is equivalent to created by.
                thisObject.ProducedBy = content.User.FirstName + " " + content.User.LastName;

                thisObject.TourLength = content.TourRunLength;

                // Set Thumbnail properties.
                var thumbnailDetail = new FileDetail {AzureID = content.ThumbnailID ?? Guid.Empty};
                thisObject.Thumbnail = thumbnailDetail;

                // Set video properties.
                var video = content.ContentRelation.FirstOrDefault(cr => cr.ContentRelationshipTypeID == (int)AssociatedContentRelationshipType.Video);
                if (video != null)
                {
                    var videoDetails = new DataDetail();
                    thisObject.Video = videoDetails.SetValuesFrom(video.Content1);
                }

                // Set associated file details.
                thisObject.AssociatedFiles = GetAssociatedFiles(content);
                
            }
        }

        /// <summary>
        /// Populates the ContentViewModel object's properties from the given ContentsView object's properties.
        /// </summary>
        /// <param name="thisObject">Current content view model on which the extension method is called</param>
        /// <param name="content">ContentInputViewModel model from which values to be read</param>
        public static void SetValuesFrom(this ContentDetails thisObject, ContentInputViewModel content)
        {
            if (thisObject != null && content != null)
            {
                // Populate the base values using the EntityViewModel's SetValuesFrom method.
                (thisObject as EntityDetails).SetValuesFrom(content);

                thisObject.DistributedBy = content.DistributedBy;
                thisObject.Citation = content.Citation;

                // Set the access type for the content
                thisObject.AccessTypeID = content.AccessTypeID;

                thisObject.TourLength = content.TourLength;

                // Set content data properties.
                if (content.IsLink)
                {
                    thisObject.ContentData = new LinkDetail(content.ContentUrl);
                }
                else
                {
                    string[] fileDetails = content.ContentFileDetail.Split('~');

                    if (fileDetails.Count() == 5)
                    {
                        string mimeType = fileDetails[3];

                        // If the content file details does not have the following details 
                        // then do not process the file.
                        DataDetail contentDetail = null;

                        if (mimeType.ToUpperInvariant().Equals(Constants.LinkMimeType.ToUpperInvariant()))
                        {
                            contentDetail = new LinkDetail(content.FileName);
                        }
                        else
                        {
                            var fileDetail = new FileDetail();

                            // Get file name and Content type.
                            fileDetail.Name = content.FileName;
                            fileDetail.ContentType = fileDetails[0].GetContentTypes();

                            // Get File size.
                            long fileSize;
                            if (long.TryParse(fileDetails[1], out fileSize))
                            {
                                fileDetail.Size = fileSize;
                            }

                            fileDetail.AzureID = content.ContentDataID;

                            // Get content mime Type.
                            fileDetail.MimeType = fileDetails[3];

                            contentDetail = fileDetail;
                        }

                        // Set Content ID if present.
                        long contentID;
                        if (long.TryParse(fileDetails[4], out contentID))
                        {
                            contentDetail.ContentID = contentID;
                        }

                        thisObject.ContentData = contentDetail;
                    }
                }

                // Set video properties.
                if (content.VideoID != Guid.Empty && content.VideoFileDetail != null)
                {
                    var video = new FileDetail();
                    string[] videoDetails = content.VideoFileDetail.Split('~');

                    if (videoDetails.Count() == 5)
                    {
                        var fileDetail = new FileDetail();

                        // Get file name and Content type.
                        fileDetail.Name = content.VideoName;
                        fileDetail.ContentType = videoDetails[0].GetContentTypes();

                        // Get File size.
                        long fileSize;
                        if (long.TryParse(videoDetails[1], out fileSize))
                        {
                            fileDetail.Size = fileSize;
                        }

                        fileDetail.AzureID = content.VideoID;

                        // Get content mime Type.
                        fileDetail.MimeType = videoDetails[3];

                        video = fileDetail;
                    }

                    thisObject.Video = video;
                }

                // Set associated file details.
                thisObject.AssociatedFiles = GetAssociatedFiles(content);
            }
        }

        /// <summary>
        /// Gets the associated files from content input view model.
        /// </summary>
        /// <param name="content">Input view model.</param>
        /// <returns>Associated files.</returns>
        private static IEnumerable<DataDetail> GetAssociatedFiles(ContentInputViewModel content)
        {
            var associatedFiles = new List<DataDetail>();
            if (content.PostedFileDetail != null)
            {
                for (int i = 0; i < content.PostedFileDetail.Count(); i++)
                {
                    var file = content.PostedFileDetail.ElementAt(i);
                    string[] fileDetails = file.Split('~');

                    if (fileDetails.Count() == 5)
                    {
                        string mimeType = fileDetails[3];

                        // If the posted file details does not have the following details 
                        // then do not process the file.
                        DataDetail contentDetail = null;

                        if (mimeType.ToUpperInvariant().Equals(Constants.LinkMimeType.ToUpperInvariant()))
                        {
                            contentDetail = new LinkDetail(content.PostedFileName.ElementAt(i));
                        }
                        else
                        {
                            var fileDetail = new FileDetail();

                            // Get and set Content type.
                            fileDetail.ContentType = fileDetails[0].GetContentTypes();

                            // Get file name and extension.
                            fileDetail.Name = string.Format(
                                System.Globalization.CultureInfo.InvariantCulture,
                                "{0}{1}",
                                content.PostedFileName.ElementAt(i),
                                fileDetails[0]);

                            // Get File size.
                            long fileSize;
                            if (long.TryParse(fileDetails[1], out fileSize))
                            {
                                fileDetail.Size = fileSize;
                            }

                            // Get Azure ID
                            Guid fileID;
                            if (Guid.TryParse(fileDetails[2], out fileID))
                            {
                                fileDetail.AzureID = fileID;
                            }

                            // Get content mime Type.
                            fileDetail.MimeType = fileDetails[3];

                            contentDetail = fileDetail;
                        }

                        // Set Content ID if present.
                        long contentID;
                        if (long.TryParse(fileDetails[4], out contentID))
                        {
                            contentDetail.ContentID = contentID;
                        }

                        associatedFiles.Add(contentDetail);
                    }
                }
            }

            return associatedFiles;
        }

        /// <summary>
        /// Gets the associated files from content input view model.
        /// </summary>
        /// <param name="content">Input view model.</param>
        /// <returns>Associated files.</returns>
        private static IEnumerable<DataDetail> GetAssociatedFiles(Content content)
        {
            var associatedFiles = new List<DataDetail>();
            if (content.ContentRelation.Count > 0)
            {
                for (int i = 0; i < content.ContentRelation.Count; i++)
                {
                    var contentRelation = content.ContentRelation.ElementAt(i);

                    if (contentRelation.ContentRelationshipTypeID == (int)AssociatedContentRelationshipType.Associated)
                    {
                        // If the posted file details does not have the following details 
                        // then do not process the file.
                        var fileDetail = new DataDetail();
                        associatedFiles.Add(fileDetail.SetValuesFrom(content.ContentRelation.ElementAt(i).Content1));
                    }
                }
            }

            return associatedFiles;
        }
    }
}
