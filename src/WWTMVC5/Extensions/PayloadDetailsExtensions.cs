//-----------------------------------------------------------------------
// <copyright file="PayloadDetailsExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using WWTMVC5.Models;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for PayloadDetails Model
    /// </summary>
    public static class PayloadDetailsExtensions
    {
        /// <summary>
        /// Initialize Payload instance
        /// </summary>
        /// <returns>PayloadDetails instance</returns>
        public static PayloadDetails InitializePayload()
        {
            var thisObject = new PayloadDetails();
            thisObject.Links = new Collection<Place>();
            thisObject.Children = new Collection<PayloadDetails>();
            thisObject.Tours = new Collection<TourModel>();
            thisObject.FolderType = "Earth";
            thisObject.Group = "Explorer";
            thisObject.Searchable = "True";
            thisObject.FolderRefreshType = "ConditionalGet";
            thisObject.CommunityType = CommunityTypes.Folder;
            thisObject.Permission = Permission.Reader;
            return thisObject;
        }

        /// <summary>
        /// Populates the PayloadDetails object's properties from the given collection of Community view object's properties.
        /// </summary>
        /// <param name="thisObject">PayloadDetails object</param>
        /// <param name="communities">Community View object</param>
        /// <returns>PayloadDetails instance</returns>
        public static PayloadDetails SetValuesFrom(this PayloadDetails thisObject, IEnumerable<CommunityDetails> communities)
        {
            if (communities != null)
            {
                if (thisObject == null)
                {
                    thisObject = InitializePayload();
                }

                foreach (var community in communities)
                {
                    var childCommunity = new PayloadDetails();
                    childCommunity.Name = community.Name;
                    childCommunity.Id = community.ID.ToString(CultureInfo.InvariantCulture);
                    childCommunity.MSRCommunityId = community.ID;
                    childCommunity.FolderType = "Earth";
                    childCommunity.Group = "Explorer";
                    childCommunity.CommunityType = community.CommunityType;
                    childCommunity.FolderRefreshType = "ConditionalGet";
                    childCommunity.Searchable = community.CommunityType == CommunityTypes.Community ? true.ToString() : false.ToString();
                    childCommunity.Thumbnail = (community.Thumbnail != null && community.Thumbnail.AzureID != Guid.Empty) ? community.Thumbnail.AzureID.ToString() : null;
                    childCommunity.Permission = community.UserPermission;

                    thisObject.Children.Add(childCommunity);
                }
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the PayloadDetails object's properties from the given collection of Community view object's properties.
        /// </summary>
        /// <param name="thisObject">PayloadDetails object</param>
        /// <param name="communities">Community View object</param>
        /// <returns>PayloadDetails instance</returns>
        public static PayloadDetails SetValuesFrom(this PayloadDetails thisObject, IEnumerable<CommunitiesView> communities)
        {
            if (communities != null)
            {
                if (thisObject == null)
                {
                    thisObject = InitializePayload();
                }

                foreach (var community in communities)
                {
                    var childCommunity = new PayloadDetails();
                    childCommunity.Name = community.CommunityName;
                    childCommunity.Id = community.CommunityID.ToString(CultureInfo.InvariantCulture);
                    childCommunity.MSRCommunityId = community.CommunityID;
                    childCommunity.FolderType = "Earth";
                    childCommunity.Group = "Explorer";
                    childCommunity.CommunityType = CommunityTypes.Community;
                    childCommunity.FolderRefreshType = "ConditionalGet";
                    childCommunity.Searchable = true.ToString();
                    childCommunity.Thumbnail = (community.ThumbnailID != null && community.ThumbnailID.Value != Guid.Empty) ? community.ThumbnailID.ToString() : null;
                    childCommunity.Permission = Permission.Reader;

                    // Permissions for Community view are hard-coded to Read as it is used for browse
                    thisObject.Children.Add(childCommunity);
                }
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the PayloadDetails object's properties from the given content collection.
        /// </summary>
        /// <param name="thisObject">PayloadDetails object</param>
        /// <param name="contentDetails">Content Details collection</param>
        /// <returns>PayloadDetails instance</returns>
        public static PayloadDetails SetValuesFrom(this PayloadDetails thisObject, IEnumerable<ContentDetails> contentDetails)
        {
            if (contentDetails != null)
            {
                if (thisObject == null)
                {
                    thisObject = InitializePayload();
                }

                foreach (var contentDetail in contentDetails)
                {
                    SetValuesFrom(thisObject, contentDetail);
                }
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the PayloadDetails object's properties from the given collection of ContentsView object's properties.
        /// </summary>
        /// <param name="thisObject">PayloadDetails object</param>
        /// <param name="contents">ContentsView objects</param>
        /// <returns>PayloadDetails instance</returns>
        public static PayloadDetails SetValuesFrom(this PayloadDetails thisObject, IEnumerable<ContentsView> contents)
        {
            if (contents != null)
            {
                if (thisObject == null)
                {
                    thisObject = InitializePayload();
                }

                foreach (var content in contents)
                {
                    if (content.Filename.EndsWith(Constants.TourFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        var tour = new TourModel();
                        tour.SetValuesFrom(content);
                        thisObject.Tours.Add(tour);
                    }
                    else if (content.Filename.EndsWith(Constants.CollectionFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        var childCollection = new PayloadDetails();
                        childCollection.Name = content.Title;
                        childCollection.Id = content.ContentAzureID.ToString();
                        childCollection.MSRComponentId = content.ContentID;
                        childCollection.Group = "Explorer";
                        childCollection.IsCollection = true;
                        childCollection.Thumbnail = (content.ThumbnailID.HasValue && content.ThumbnailID != Guid.Empty) ? content.ThumbnailID.ToString() : null;

                        // Permissions for Content view are hard-coded to Read as it is used for browse
                        childCollection.Permission = Permission.Reader;
                        thisObject.Children.Add(childCollection);
                    }
                    else
                    {
                        var place = new Place();
                        place.SetValuesFrom(content);
                        thisObject.Links.Add(place);
                    }
                }
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the PayloadDetails object's properties from the given Content object's properties.
        /// </summary>
        /// <param name="thisObject">PayloadDetails object</param>
        /// <param name="contentDetail">Content Details object</param>
        /// <returns>PayloadDetails instance</returns>
        public static PayloadDetails SetValuesFrom(this PayloadDetails thisObject, ContentDetails contentDetail)
        {
            if (contentDetail != null)
            {
                if (thisObject == null)
                {
                    thisObject = InitializePayload();
                }

                if (contentDetail.ContentData is FileDetail)
                {
                    var fileDetail = contentDetail.ContentData as FileDetail;
                    if (contentDetail.ContentData.ContentType == ContentTypes.Tours)
                    {
                        var tour = new TourModel();
                        tour.SetValuesFrom(contentDetail);
                        thisObject.Tours.Add(tour);
                    }
                    else if (contentDetail.ContentData.ContentType == ContentTypes.Wtml)
                    {
                        var childCollection = new PayloadDetails();
                        childCollection.Name = contentDetail.Name;
                        childCollection.Id = fileDetail.AzureID.ToString();
                        childCollection.MSRComponentId = contentDetail.ID;
                        childCollection.Group = "Explorer";
                        childCollection.IsCollection = true;
                        childCollection.Thumbnail = (contentDetail.Thumbnail != null && contentDetail.Thumbnail.AzureID != Guid.Empty) ? contentDetail.Thumbnail.AzureID.ToString() : null;
                        childCollection.Permission = contentDetail.UserPermission;
                        thisObject.Children.Add(childCollection);
                    }
                    else
                    {
                        var place = new Place();
                        place.SetValuesFrom(contentDetail);
                        thisObject.Links.Add(place);
                    }
                }
                else
                {
                    var place = new Place();
                    place.SetValuesFrom(contentDetail);
                    thisObject.Links.Add(place);
                }
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the PayloadDetails object's properties from the given collection of Categories object's properties.
        /// </summary>
        /// <param name="thisObject">PayloadDetails object</param>
        /// <param name="categories">Categories object</param>
        /// <returns>PayloadDetails instance</returns>
        public static PayloadDetails SetValuesFrom(this PayloadDetails thisObject, SelectList categories)
        {
            if (categories != null)
            {
                if (thisObject == null)
                {
                    thisObject = InitializePayload();
                }

                foreach (var category in categories)
                {
                    var childCategory = new PayloadDetails();
                    childCategory.Name = category.Text;
                    childCategory.Id = category.Value;
                    childCategory.FolderType = "Earth";
                    childCategory.Group = "Explorer";
                    childCategory.CommunityType = CommunityTypes.Folder;
                    childCategory.FolderRefreshType = "ConditionalGet";
                    childCategory.Searchable = true.ToString();

                    // TODO : Thumbnail for Each category
                    // childCommunity.Thumbnail = (community.ThumbnailID != null && community.ThumbnailID.Value != Guid.Empty) ? community.ThumbnailID.ToString() : null;
                    childCategory.Permission = Permission.Reader;

                    // Permissions for Community view are hard-coded to Read as it is used for browse
                    thisObject.Children.Add(childCategory);
                }
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the Tour object's properties from the given content.
        /// </summary>
        /// <param name="thisObject">Tour object</param>
        /// <param name="contentDetail">content Detail object</param>
        /// <returns>Tour instance</returns>
        public static TourModel SetValuesFrom(this TourModel thisObject, ContentDetails contentDetail)
        {
            if (contentDetail != null)
            {
                if (thisObject == null)
                {
                    thisObject = new TourModel();
                }

                var fileDetail = contentDetail.ContentData as FileDetail;

                thisObject.Title = contentDetail.Name;
                thisObject.TourUrl = fileDetail.AzureID.ToString();
                thisObject.ID = fileDetail.AzureID.ToString();
                thisObject.MSRComponentId = contentDetail.ID;
                thisObject.Description = HttpUtility.HtmlDecode(contentDetail.Description).GetTextFromHtmlString();

                thisObject.ThumbnailUrl = (contentDetail.Thumbnail != null && contentDetail.Thumbnail.AzureID != Guid.Empty) ? contentDetail.Thumbnail.AzureID.ToString() : null;
                thisObject.LengthInSecs = contentDetail.TourLength;
                thisObject.AverageRating = contentDetail.AverageRating.ToString(CultureInfo.InvariantCulture);
                thisObject.Permission = contentDetail.UserPermission;

                // New fields
                thisObject.Author = HttpUtility.HtmlDecode(contentDetail.DistributedBy).GetTextFromHtmlString();
                thisObject.OrganizationName = string.Empty;
                thisObject.OrganizationUrl = string.Empty;
                thisObject.RelatedTours = string.Empty;
                thisObject.AuthorURL = contentDetail.CreatedByID.ToString(CultureInfo.InvariantCulture);
                thisObject.AuthorImageUrl = thisObject.AuthorURL;

                double tourDuration;
                if (Double.TryParse(contentDetail.TourLength, out tourDuration))
                {
                    var timeSpan = TimeSpan.FromSeconds(tourDuration);
                    thisObject.TourDuration = string.Format(CultureInfo.InvariantCulture, "{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                }
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the Place object's properties from the given content.
        /// </summary>
        /// <param name="thisObject">Place object</param>
        /// <param name="contentDetail">content Details object</param>
        /// <returns>Place instance</returns>
        public static Place SetValuesFrom(this Place thisObject, ContentDetails contentDetail)
        {
            if (contentDetail != null)
            {
                if (thisObject == null)
                {
                    thisObject = new Place();
                }

                thisObject.Name = contentDetail.Name;
                thisObject.MSRComponentId = contentDetail.ID;

                thisObject.FileType = contentDetail.ContentData.ContentType;
                if (contentDetail.ContentData is LinkDetail)
                {
                    var linkDetail = contentDetail.ContentData as LinkDetail;
                    thisObject.ContentLink = linkDetail.ContentUrl;
                    thisObject.ContentAzureID = string.Empty;
                }
                else
                {
                    var fileDetail = contentDetail.ContentData as FileDetail;
                    thisObject.ContentLink = string.Empty;
                    thisObject.ContentAzureID = fileDetail.AzureID.ToString();
                }

                thisObject.Url = contentDetail.ContentData.Name;
                thisObject.Thumbnail = (contentDetail.Thumbnail != null && contentDetail.Thumbnail.AzureID != Guid.Empty) ? contentDetail.Thumbnail.AzureID.ToString() : null;
                thisObject.Permission = contentDetail.UserPermission;
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the Tour object's properties from the given content view.
        /// </summary>
        /// <param name="thisObject">Tour object</param>
        /// <param name="contentsView">content view object</param>
        /// <returns>Tour instance</returns>
        public static TourModel SetValuesFrom(this TourModel thisObject, ContentsView contentsView)
        {
            if (contentsView != null)
            {
                if (thisObject == null)
                {
                    thisObject = new TourModel();
                }

                thisObject.Title = contentsView.Title;
                thisObject.TourUrl = contentsView.ContentAzureID.ToString();
                thisObject.ID = contentsView.ContentAzureID.ToString();
                thisObject.MSRComponentId = contentsView.ContentID;
                thisObject.Description = contentsView.Description;
                thisObject.Author = contentsView.DistributedBy;
                thisObject.ThumbnailUrl = (contentsView.ThumbnailID.HasValue && contentsView.ThumbnailID != Guid.Empty) ? contentsView.ThumbnailID.ToString() : null;
                thisObject.LengthInSecs = contentsView.TourRunLength;
                thisObject.AverageRating = contentsView.AverageRating.HasValue ? contentsView.AverageRating.ToString() : "0";

                // Permissions for Content view are hard-coded to Read as it is used for browse
                thisObject.Permission = Permission.Reader;
            }

            return thisObject;
        }

        /// <summary>
        /// Populates the Place object's properties from the given content view.
        /// </summary>
        /// <param name="thisObject">Place object</param>
        /// <param name="contentsView">content view object</param>
        /// <returns>Place instance</returns>
        public static Place SetValuesFrom(this Place thisObject, ContentsView contentsView)
        {
            if (contentsView != null)
            {
                if (thisObject == null)
                {
                    thisObject = new Place();
                }

                thisObject.Name = contentsView.Title;
                thisObject.MSRComponentId = contentsView.ContentID;

                thisObject.FileType = contentsView.TypeID.ToEnum<int, ContentTypes>(ContentTypes.Generic);
                thisObject.ContentLink = thisObject.FileType == ContentTypes.Link ? contentsView.ContentUrl : string.Empty;
                thisObject.ContentAzureID = thisObject.FileType == ContentTypes.Link ? string.Empty : contentsView.ContentAzureID.ToString();

                thisObject.Url = contentsView.Filename;
                thisObject.Thumbnail = (contentsView.ThumbnailID.HasValue && contentsView.ThumbnailID != Guid.Empty) ? contentsView.ThumbnailID.ToString() : null;

                // Permissions for Content view are hard-coded to Read as it is used for browse
                thisObject.Permission = Permission.Reader;
            }

            return thisObject;
        }
    }
}