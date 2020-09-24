//-----------------------------------------------------------------------
// <copyright file="EntityViewModelExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using WWTMVC5.Models;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for EntityViewModel.
    /// </summary>
    public static class EntityViewModelExtensions
    {
        /// <summary>
        /// Populates the EntityViewModel object's properties from the given EntityDetail object's properties.
        /// </summary>
        /// <param name="thisObject">Current entity view model on which the extension method is called</param>
        /// <param name="entityDetails">EntityDetails model from which values to be read</param>
        /// <returns>Values populated EntityViewModel instance</returns>
        public static EntityViewModel SetValuesFrom(this EntityViewModel thisObject, EntityDetails entityDetails)
        {
            if (entityDetails != null)
            {
                if (thisObject == null)
                {
                    thisObject = new EntityViewModel();
                }

                thisObject.Id = entityDetails.ID;
                thisObject.Name = entityDetails.Name;

                // Parse the category string
                thisObject.Category = entityDetails.CategoryID.ToEnum<int, CategoryType>(CategoryType.All);
                thisObject.AccessType = (AccessType)entityDetails.AccessTypeID;

                thisObject.ParentId = entityDetails.ParentID;
                thisObject.ParentName = entityDetails.ParentName;
                thisObject.ParentType = entityDetails.ParentType;
                thisObject.UserPermission = entityDetails.UserPermission;

                if (entityDetails.Tags != null)
                {
                    thisObject.Tags = entityDetails.Tags;
                }

                thisObject.Rating = (double)entityDetails.AverageRating;
                thisObject.RatedPeople = entityDetails.RatedPeople;
                if (entityDetails.Thumbnail != null)
                {
                    thisObject.ThumbnailID = entityDetails.Thumbnail.AzureID;
                }

                thisObject.Producer = entityDetails.ProducedBy;
                thisObject.ProducerId = entityDetails.CreatedByID;
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the EntityViewModel object's properties from the given CommunityDetails object's properties.
        /// </summary>
        /// <param name="thisObject">Current entity view model on which the extension method is called</param>
        /// <param name="communityDetails">CommunityDetails model from which values to be read</param>
        /// <returns>Values populated EntityViewModel instance</returns>
        public static EntityViewModel SetValuesFrom(this EntityViewModel thisObject, CommunityDetails communityDetails)
        {
            if (communityDetails != null)
            {
                if (thisObject == null)
                {
                    thisObject = new EntityViewModel();
                }

                thisObject.Entity = communityDetails.CommunityType == CommunityTypes.Community ? EntityType.Community : EntityType.Folder;

                // Populate the base values using the EntityViewModel's SetValuesFrom method which take EntityDetails as input.
                thisObject.SetValuesFrom(communityDetails as EntityDetails);
            }

            return thisObject;
        }

        /// <summary>
        /// Gets the thumbnail of the given entityView.
        /// </summary>
        /// <param name="thisObject">Current entity view model on which the extension method is called</param>
        public static string GetThumbnailLink(this EntityViewModel thisObject)
        {
            string thumbnailLink = string.Empty;

            if (thisObject != null)
            {
                if (thisObject.ThumbnailID.HasValue && !Guid.Empty.Equals(thisObject.ThumbnailID.Value))
                {
                    thumbnailLink = "/file/thumbnail/" + thisObject.ThumbnailID;
                }
                else
                {
                    if (thisObject.Entity == EntityType.Community)
                    {
                        thumbnailLink = "/Content/Images/defaultcommunitythumbnail.png";
                    }
                    else if (thisObject.Entity == EntityType.Folder)
                    {
                        thumbnailLink = "/Content/Images/defaultfolderthumbnail.png";
                    }
                    else if (thisObject.Entity == EntityType.Content)
                    {
                        thumbnailLink = "/Content/Images/default" + Enum.GetName(typeof(ContentTypes), thisObject.ContentType) + "thumbnail.png";
                    }
                }
            }

            return thumbnailLink;
        }
    }
}
