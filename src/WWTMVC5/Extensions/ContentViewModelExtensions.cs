//-----------------------------------------------------------------------
// <copyright file="ContentViewModelExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using AutoMapper;
using WWTMVC5.Models;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for ContentViewModel.
    /// </summary>
    public static class ContentViewModelExtensions
    {
        /// <summary>
        /// Populates the ContentViewModel object's properties from the given ContentDetails object's properties.
        /// </summary>
        /// <param name="thisObject">Current content view model on which the extension method is called</param>
        /// <param name="content">ContentDetails model from which values to be read</param>
        /// <returns>Values populated ContentViewModel instance</returns>
        public static ContentViewModel SetValuesFrom(this ContentViewModel thisObject, ContentDetails content)
        {
            if (content != null)
            {
                if (thisObject == null)
                {
                    thisObject = new ContentViewModel();
                }

                // Populate the base values using the EntityViewModel's SetValuesFrom method.
                (thisObject as EntityViewModel).SetValuesFrom(content);

                thisObject.Description = content.Description;
                thisObject.Citation = content.Citation;
                thisObject.IsOffensive = content.IsOffensive;

                if (content.ContentData is LinkDetail)
                {
                    var linkDetail = content.ContentData as LinkDetail;
                    thisObject.ContentUrl = linkDetail.ContentUrl;
                }
                else if (content.ContentData is FileDetail)
                {
                    var fileDetail = content.ContentData as FileDetail;
                    thisObject.ContentAzureID = fileDetail.AzureID;
                    thisObject.Size = fileDetail.Size;
                }

                if (content.ContentData != null)
                {
                    // Update content properties.
                    thisObject.FileName = content.ContentData.Name;
                    thisObject.ContentType = content.ContentData.ContentType;
                    thisObject.IsLink = content.ContentData.ContentType == ContentTypes.Link;
                }
                else
                {
                    // When list of contents of a community are rendered, they will not have ContentData since it is not required in list view.
                    ContentTypes type = content.TypeID.ToEnum<int, ContentTypes>(ContentTypes.Generic);

                    // Update content properties.
                    thisObject.ContentType = type;
                    thisObject.IsLink = (type == ContentTypes.Link);
                }

                thisObject.Entity = EntityType.Content;

                var videoData = content.Video as FileDetail;
                if (videoData != null)
                {
                    // Video field takes precedence over the content which is a video.
                    thisObject.VideoID = videoData.AzureID;
                    thisObject.VideoName = videoData.Name;
                }
                else if (thisObject.ContentType == ContentTypes.Video)
                {
                    // Show video link if the content is a video.
                    thisObject.VideoID = thisObject.ContentAzureID.Value;
                    thisObject.VideoName = thisObject.FileName;
                }

                DateTime lastUpdated = content.LastUpdatedDatetime.HasValue ? content.LastUpdatedDatetime.Value : content.CreatedDatetime.Value;
                thisObject.LastUpdated = lastUpdated.GetFormattedDifference(DateTime.UtcNow);

                thisObject.DistributedBy = content.DistributedBy;
                thisObject.RatedPeople = content.RatedPeople;
                thisObject.DownloadCount = content.DownloadCount;

                if (content.AssociatedFiles != null)
                {
                    var associatedFiles = new List<AssociatedFileViewModel>();
                    foreach (var associatedFile in content.AssociatedFiles)
                    {
                        var file = new AssociatedFileViewModel();

                        var fileDetails = associatedFile as FileDetail;
                        if (fileDetails != null)
                        {
                            Mapper.Map(fileDetails, file);
                            file.IsLink = false;
                        }
                        else
                        {
                            var linkDetails = associatedFile as LinkDetail;
                            if (linkDetails != null)
                            {
                                Mapper.Map(linkDetails, file);
                                file.IsLink = true;
                            }
                        }

                        associatedFiles.Add(file);
                    }
                    thisObject.AssociatedFiles = associatedFiles;
                }
            }

            return thisObject;
        }
    }
}
