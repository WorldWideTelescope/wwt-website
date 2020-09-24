//-----------------------------------------------------------------------
// <copyright file="PivotCollectionActionExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Extensions
{
    public static class PivotCollectionActionExtensions
    {
        /// <summary>
        /// Populates the PivotCollectionAction object's properties from the given EntityViewModel object's properties.
        /// </summary>
        /// <param name="thisObject">PivotCollectionAction object</param>
        /// <param name="entityViewModelResults">EntityViewModel list</param>
        /// <param name="imageBase">Image Base string</param>
        /// <returns>PivotCollectionAction instance</returns>
        //public static PivotCollectionActionResult<DeepZoomViewModel> SetValuesFrom(this PivotCollectionActionResult<DeepZoomViewModel> thisObject, IEnumerable<DeepZoomViewModel> entityViewModelResults, string imageBase)
        //{
        //    if (entityViewModelResults != null)
        //    {
        //        if (thisObject == null)
        //        {
        //            thisObject = new PivotCollectionActionResult<DeepZoomViewModel>("Results");
        //        }

        //        thisObject.ItemName = m => m.Name;
        //        thisObject.Id = m => m.Id;
        //        thisObject.ItemType = m => m.Entity;
        //        thisObject.Description = m => m.Description;
        //        thisObject.Items = entityViewModelResults;
        //        thisObject.ImageBase = imageBase;

        //        SetFacetCategories(thisObject);
        //    }

        //    return thisObject;
        //}

        ///// <summary>
        ///// Sets facet categories on DeepZoomViewModel object
        ///// </summary>
        ///// <param name="thisObject">DeepZoom View Model</param>
        //private static void SetFacetCategories(PivotCollectionActionResult<DeepZoomViewModel> thisObject)
        //{
        //    thisObject.FacetCategories.Add(new FacetCategory<DeepZoomViewModel> { Name = Properties.Resources.SearchRatingFilter, FacetDataType = FacetType.String, Action = m => GetRating(m.Rating), IsWordWheelVisible = true, IsFilterVisible = true, IsMetaDataVisible = true });
        //    thisObject.FacetCategories.Add(new FacetCategory<DeepZoomViewModel> { Name = Properties.Resources.SearchCategoriesFilter, FacetDataType = FacetType.String, Action = m => Resources.ResourceManager.GetString(m.Category.ToString()), IsWordWheelVisible = true, IsFilterVisible = true, IsMetaDataVisible = true });
        //    thisObject.FacetCategories.Add(new FacetCategory<DeepZoomViewModel> { Name = Properties.Resources.SearchDateFilter, FacetDataType = FacetType.DateTime, Action = m => m.LastUpdated, IsWordWheelVisible = true, IsFilterVisible = true, IsMetaDataVisible = true });
        //    thisObject.FacetCategories.Add(new FacetCategory<DeepZoomViewModel> { Name = Properties.Resources.SearchDistributedByFilter, FacetDataType = FacetType.String, Action = m => m.DistributedBy, IsWordWheelVisible = true, IsFilterVisible = true, IsMetaDataVisible = true });
        //    thisObject.FacetCategories.Add(new FacetCategory<DeepZoomViewModel> { Name = Properties.Resources.SearchContentTypeFilter, FacetDataType = FacetType.String, Action = m => m.FileType.GetName(), IsWordWheelVisible = true, IsFilterVisible = true, IsMetaDataVisible = true });
        //    thisObject.FacetCategories.Add(new FacetCategory<DeepZoomViewModel> { Name = Properties.Resources.SearchViewDownloadFilter, FacetDataType = FacetType.Link, Href = m => m.DownloadLink.ToString(), IsWordWheelVisible = true, IsFilterVisible = false, IsMetaDataVisible = true });
        //    thisObject.FacetCategories.Add(new FacetCategory<DeepZoomViewModel> { Name = Properties.Resources.SearchTagsFilter, FacetDataType = FacetType.LongString, Action = m => m.Tags, IsWordWheelVisible = true, IsFilterVisible = false, IsMetaDataVisible = true });
        //    thisObject.FacetCategories.Add(new FacetCategory<DeepZoomViewModel> { Name = Properties.Resources.SearchCitationFilter, FacetDataType = FacetType.LongString, Action = m => m.Citation, IsWordWheelVisible = true, IsFilterVisible = false, IsMetaDataVisible = true });
        //}

        /// <summary>
        /// Returns Rating value
        /// </summary>
        /// <param name="averageRating">average Rating value</param>
        /// <returns>Returns Rating  for Pivot Viewer</returns>
        private static string GetRating(double averageRating)
        {
            double returnValue = 0;

            if (averageRating > 0 & averageRating <= 0.5)
            {
                returnValue = 0.5;
            }
            else if (averageRating > 1 & averageRating <= 1.5)
            {
                returnValue = 1.5;
            }
            else if (averageRating > 2 & averageRating <= 2.5)
            {
                returnValue = 2.5;
            }
            else if (averageRating > 3 & averageRating <= 3.5)
            {
                returnValue = 3.5;
            }
            else if (averageRating > 4 & averageRating <= 4.5)
            {
                returnValue = 4.5;
            }
            else 
            {
                returnValue = Math.Round(averageRating, MidpointRounding.AwayFromZero);
            }

            return returnValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}