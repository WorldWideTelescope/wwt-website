//-----------------------------------------------------------------------
// <copyright file="EntityDetailsExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using AutoMapper;
using WWTMVC5.Models;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for entity details.
    /// </summary>
    public static class EntityDetailsExtensions
    {
        /// <summary>
        /// Populates the EntityDetails object's properties from the given Content's object's properties.
        /// </summary>
        /// <param name="thisObject">Current entity details model on which the extension method is called</param>
        /// <param name="content">Content model from which values to be read</param>
        public static void SetValuesFrom(this EntityDetails thisObject, Content content)
        {
            if (content != null)
            {
                if (thisObject == null)
                {
                    thisObject = new EntityDetails();
                }

                thisObject.Name = content.Title;
                thisObject.ID = content.ContentID;
                thisObject.Description = content.Description;
                thisObject.IsOffensive = content.IsOffensive.HasValue ? content.IsOffensive.Value : false;

                // Set parent details.
                var parent = content.CommunityContents.FirstOrDefault();
                if (parent != null)
                {
                    thisObject.ParentName = parent.Community.Name;
                    thisObject.ParentID = parent.Community.CommunityID;
                    thisObject.ParentType = parent.Community.CommunityTypeID.ToEnum<int, CommunityTypes>(CommunityTypes.None);
                }

                thisObject.CategoryID = content.CategoryID;
                thisObject.CreatedByID = content.CreatedByID;
                thisObject.TypeID = content.TypeID;
                thisObject.Thumbnail = new FileDetail() { AzureID = content.ThumbnailID.HasValue ? content.ThumbnailID.Value : Guid.Empty };

                if (content.AccessType != null)
                {
                    thisObject.AccessTypeName = content.AccessType.Name;
                }

                thisObject.CreatedDatetime = content.CreatedDatetime;
                thisObject.LastUpdatedDatetime = content.ModifiedDatetime;

                if (content.ContentRatings.Count() > 0)
                {
                    thisObject.AverageRating = content.ContentRatings.Average(rating => rating.Rating);
                    thisObject.RatedPeople = content.ContentRatings.Count();
                }

                var tags = content.ContentTags.Select(tag => tag.Tag.Name);

                if (tags.Count() > 0)
                {
                    thisObject.Tags = string.Join(", ", tags.ToList());
                }

                thisObject.AccessTypeID = content.AccessTypeID.HasValue ? content.AccessTypeID.Value : 0;
            }
        }

        /// <summary>
        /// Populates the EntityDetails object's properties from the given Community object's properties.
        /// </summary>
        /// <param name="thisObject">Current entity details model on which the extension method is called</param>
        /// <param name="community">Community model from which values to be read</param>
        public static void SetValuesFrom(this EntityDetails thisObject, Community community)
        {
            if (thisObject != null && community != null)
            {
                Mapper.Map(community, thisObject as CommunityDetails);

                // Set parent details.
                var parent = community.CommunityRelation1.FirstOrDefault();
                if (parent != null)
                {
                    thisObject.ParentName = parent.Community.Name;
                    thisObject.ParentID = parent.Community.CommunityID;
                    thisObject.ParentType = parent.Community.CommunityTypeID.ToEnum<int, CommunityTypes>(CommunityTypes.None);
                }

                if (community.CommunityRatings.Count() > 0)
                {
                    thisObject.AverageRating = community.CommunityRatings.Average(rating => rating.Rating);
                    thisObject.RatedPeople = community.CommunityRatings.Count();
                }

                var tags = community.CommunityTags.Select(tag => tag.Tag.Name);

                if (tags.Count() > 0)
                {
                    thisObject.Tags = string.Join(", ", tags.ToList());
                }

                if (community.User != null)
                {
                    // Produced by is equivalent to created by.
                    thisObject.ProducedBy = community.User.FirstName + " " + community.User.LastName;
                }

                // Set Thumbnail properties.
                var thumbnailDetail = new FileDetail();
                thumbnailDetail.AzureID = community.ThumbnailID.HasValue ? community.ThumbnailID.Value : Guid.Empty;
                thisObject.Thumbnail = thumbnailDetail;
            }
        }

        /// <summary>
        /// Populates the EntityDetails object's properties from the given Content's object's properties.
        /// </summary>
        /// <param name="thisObject">Current entity details model on which the extension method is called</param>
        /// <param name="content">ContentInputViewModel model from which values to be read</param>
        public static void SetValuesFrom(this EntityDetails thisObject, EntityInputBaseViewModel content)
        {
            if (content != null)
            {
                if (thisObject == null)
                {
                    thisObject = new EntityDetails();
                }

                thisObject.Name = content.Name;
                thisObject.ID = content.ID.HasValue ? content.ID.Value : 0;
                thisObject.Description = content.Description;
                thisObject.CategoryID = content.CategoryID;
                thisObject.CreatedByID = content.OwnerID;

                // Set parent properties.
                if (content.ParentID.HasValue)
                {
                    thisObject.ParentID = content.ParentID.Value;
                }

                // Set thumbnail properties.
                thisObject.Thumbnail = new FileDetail() { AzureID = content.ThumbnailID };

                thisObject.Tags = content.Tags;
            }
        }
    }
}