//-----------------------------------------------------------------------
// <copyright file="DeepZoomViewModelExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using WWTMVC5.Models;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for EntityViewModel.
    /// </summary>
    public static class DeepZoomViewModelExtensions
    {
        /// <summary>
        /// Populates the EntityViewModel object's properties from the given CommunitiesView object's properties.
        /// </summary>
        /// <param name="thisObject">Current entity view model on which the extension method is called</param>
        /// <param name="community">CommunitiesView model from which values to be read</param>
        /// <returns>Values populated EntityViewModel instance</returns>
        public static DeepZoomViewModel SetValuesFrom(this DeepZoomViewModel thisObject, CommunitiesView community)
        {
            if (community != null)
            {
                if (thisObject == null)
                {
                    thisObject = new DeepZoomViewModel();
                }

                thisObject.Id = community.CommunityID;
                thisObject.Name = community.CommunityName;
                thisObject.Description = community.Description;
                thisObject.LastUpdated = Convert.ToDateTime(community.LastUpdatedDatetime, CultureInfo.CurrentCulture).ToString("s", CultureInfo.CurrentCulture);
                thisObject.DistributedBy = community.DistributedBy;
                thisObject.Tags = community.Tags != null ? community.Tags : string.Empty;

                thisObject.FileType = ContentTypes.None;

                // Parse the category string
                CategoryType category = thisObject.Category = CategoryType.All;
                if (Enum.TryParse<CategoryType>(community.CategoryName, true, out category))
                {
                    thisObject.Category = category;
                }

                thisObject.Entity = community.CommunityTypeID == 1 ? EntityType.Community : EntityType.Folder;

                thisObject.Rating = community.AverageRating.HasValue ? (double)community.AverageRating : 0;
                thisObject.ThumbnailID = community.ThumbnailID;
                thisObject.FileName = string.Format(CultureInfo.InvariantCulture, Constants.SignUpFileNameFormat, community.CommunityName);
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the EntityViewModel object's properties from the given ContentsView object's properties.
        /// </summary>
        /// <param name="thisObject">Current entity view model on which the extension method is called</param>
        /// <param name="content">ContentsView model from which values to be read</param>
        /// <returns>Values populated EntityViewModel instance</returns>
        public static DeepZoomViewModel SetValuesFrom(this DeepZoomViewModel thisObject, ContentsView content)
        {
            if (content != null)
            {
                if (thisObject == null)
                {
                    thisObject = new DeepZoomViewModel();
                }

                thisObject.Id = content.ContentID;
                thisObject.Description = content.Description;
                thisObject.LastUpdated = Convert.ToDateTime(content.LastUpdatedDatetime, CultureInfo.CurrentCulture).ToString("s", CultureInfo.CurrentCulture);
                thisObject.DistributedBy = content.DistributedBy;

                thisObject.Tags = content.Tags != null ? content.Tags : string.Empty;
                thisObject.Citation = content.Citation;
                thisObject.FileType = content.TypeID.ToEnum<int, ContentTypes>(ContentTypes.Generic);
                thisObject.ContentLink = thisObject.FileType == ContentTypes.Link ? content.ContentUrl : string.Empty;
                thisObject.ContentAzureID = thisObject.FileType == ContentTypes.Link ? Guid.Empty : content.ContentAzureID;

                thisObject.Name = content.Title;
                thisObject.Category = content.CategoryID.ToEnum<int, CategoryType>(CategoryType.All);
                thisObject.Entity = EntityType.Content;

                thisObject.Rating = content.AverageRating.HasValue ? (double)content.AverageRating : 0;
                thisObject.ThumbnailID = content.ThumbnailID;
                thisObject.ContentAzureID = content.ContentAzureID;
                thisObject.FileName = content.Filename;
            }

            return thisObject;
        }

        /// <summary>
        /// Get Deep Zoom Source item URL string from the DeepZoom View model
        /// </summary>
        /// <param name="thisObject">EntityViewModel instance</param>
        /// <returns>Deep Zoom Source item URL</returns>
        public static string GetDeepZoomItemSource(this DeepZoomViewModel thisObject)
        {
            string thumbnail = "defaultcommunitythumbnail.dzi";
            if (thisObject != null)
            {
                //// 0 GUID is used for Content
                if (thisObject.ThumbnailID.HasValue && !Guid.Empty.Equals(thisObject.ThumbnailID.Value))
                {
                    thumbnail = thisObject.ThumbnailID.Value.ToString() + ".dzi";
                }
                else if (thisObject.Entity == EntityType.Content)
                {
                    thumbnail = "default" + Enum.GetName(typeof(ContentTypes), thisObject.FileType) + "thumbnail.dzi";
                }
            }

            return thumbnail;
        }
    }
}