//-----------------------------------------------------------------------
// <copyright file="ContentExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WWTMVC5.Models;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Contents.
    /// </summary>
    public static class ContentExtensions
    {
        /// <summary>
        /// Populates the Content object's properties from the given ContentDetails object's properties.
        /// </summary>
        /// <param name="thisObject">Current Content instance on which the extension method is called</param>
        /// <param name="contentDetails">ContentDetails model from which values to be read</param>
        public static void SetValuesFrom(this Content thisObject, ContentDetails contentDetails)
        {
            if (contentDetails != null && thisObject != null)
            {
                thisObject.Title = contentDetails.Name;
                thisObject.Description = contentDetails.Description;
                thisObject.Citation = contentDetails.Citation;
                thisObject.DistributedBy = contentDetails.DistributedBy;
                thisObject.AccessTypeID = contentDetails.AccessTypeID;
                thisObject.CategoryID = contentDetails.CategoryID;
                thisObject.TourRunLength = contentDetails.TourLength;

                thisObject.SetValuesFrom(contentDetails.ContentData);
            }
        }
        
        /// <summary>
        /// Create associated content from file details.
        /// </summary>
        /// <param name="thisObject">Current Content instance on which the extension method is called</param>
        /// <param name="contentDetails">Content details.</param>
        /// <param name="dataDetails">Associated file details.</param>
        public static void SetValuesFrom(this Content thisObject, EntityDetails contentDetails, DataDetail dataDetails)
        {
            if (thisObject != null && contentDetails != null && dataDetails != null)
            {
                thisObject.SetValuesFrom(dataDetails);

                thisObject.Title = dataDetails.Name;
                thisObject.Description = dataDetails.Name;
                thisObject.CategoryID = contentDetails.CategoryID;
                thisObject.AccessTypeID = contentDetails.AccessTypeID;
                thisObject.CreatedByID = contentDetails.CreatedByID;
                thisObject.CreatedDatetime = thisObject.ModifiedDatetime = contentDetails.CreatedDatetime;

                thisObject.IsSearchable = false;
                thisObject.IsDeleted = false;
                thisObject.IsOffensive = false;
            }
        }

        /// <summary>
        /// Populates the Content object's properties from the given DataDetail object's properties.
        /// </summary>
        /// <param name="thisObject">Current Content instance on which the extension method is called</param>
        /// <param name="dataDetails">DataDetail model from which values to be read</param>
        public static void SetValuesFrom(this Content thisObject, DataDetail dataDetails)
        {
            if (dataDetails != null && thisObject != null)
            {
                // Content Type ID.
                if ((int)dataDetails.ContentType != 0)
                {
                    thisObject.TypeID = (int)dataDetails.ContentType;
                }

                // Set File name
                if (!string.IsNullOrWhiteSpace(dataDetails.Name))
                {
                    thisObject.Filename = dataDetails.Name;
                }

                var fileDetails = dataDetails as FileDetail;
                if (fileDetails != null)
                {
                    thisObject.SetValuesFrom(fileDetails);
                }
                else
                {
                    var linkDetails = dataDetails as LinkDetail;
                    if (linkDetails != null)
                    {
                        thisObject.SetValuesFrom(linkDetails);
                    }
                }
            }
        }

        /// <summary>
        /// Populates the Content object's properties from the given FileDetail object's properties.
        /// </summary>
        /// <param name="thisObject">Current Content instance on which the extension method is called</param>
        /// <param name="fileDetails">FileDetail model from which values to be read</param>
        public static void SetValuesFrom(this Content thisObject, FileDetail fileDetails)
        {
            if (fileDetails != null && thisObject != null)
            {
                if (fileDetails.Size > 0)
                {
                    thisObject.Size = fileDetails.Size;
                }

                // Update Azure ID.
                thisObject.ContentAzureID = fileDetails.AzureID;
                thisObject.ContentAzureURL = fileDetails.AzureURL;
            }
        }

        /// <summary>
        /// Populates the Content object's properties from the given LinkDetail object's properties.
        /// </summary>
        /// <param name="thisObject">Current Content instance on which the extension method is called</param>
        /// <param name="linkDetails">LinkDetail model from which values to be read</param>
        public static void SetValuesFrom(this Content thisObject, LinkDetail linkDetails)
        {
            if (linkDetails != null && thisObject != null)
            {
                thisObject.ContentUrl = linkDetails.ContentUrl;
            }
        }

        /// <summary>
        /// Delete all existing tags which are not part of the new tags list.
        /// </summary>
        /// <param name="thisObject">Current Content instance on which the extension method is called</param>
        /// <param name="tags">Comma separated tags string</param>
        public static void RemoveTags(this Content thisObject, string tags)
        {
            if (thisObject != null)
            {
                if (!string.IsNullOrWhiteSpace(tags))
                {
                    IEnumerable<string> tagsArray = tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
                    if (tagsArray != null && tagsArray.Count() > 0)
                    {
                        var removeTags = from ct in thisObject.ContentTags
                                         where !tagsArray.Contains(ct.Tag.Name)
                                         select ct;

                        foreach (var item in removeTags.ToList())
                        {
                            thisObject.ContentTags.Remove(item);
                        }
                    }
                    else if (thisObject.ContentTags.Count > 0)
                    {
                        thisObject.ContentTags.Clear();
                    }
                }
                else if (thisObject.ContentTags.Count > 0)
                {
                    thisObject.ContentTags.Clear();
                }
            }
        }
    }
}