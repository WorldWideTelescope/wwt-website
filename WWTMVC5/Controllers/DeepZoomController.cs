//-----------------------------------------------------------------------
// <copyright file="DeepZoomController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Controllers
{
    public class DeepZoomController : ControllerBase
    {
        /// <summary>
        ///  Default Deep Zoom Item Xml
        /// </summary>
        private const string DeepZoomItemXml = @"<?xml version=""1.0"" encoding=""utf-8""?><Image TileSize=""256"" Overlap=""1"" Format=""jpg"" ServerFormat=""Default"" xmlns=""http://schemas.microsoft.com/deepzoom/2009""><Size Width=""256"" Height=""256"" /></Image>";

        /// <summary>
        /// Instance of blob Service
        /// </summary>
        private IBlobService blobService;

        /// <summary>
        /// Instance of Search Service
        /// </summary>
        private ISearchService searchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeepZoomController"/> class.
        /// </summary>
        /// <param name="blobService">
        /// Instance of a blob service.
        /// </param>
        /// <param name="searchService">
        /// Instance of a search service.
        /// </param>
        /// <param name="profileService">
        /// Instance of a profile service.
        /// </param>
        public DeepZoomController(IBlobService blobService, ISearchService searchService, IProfileService profileService)
            : base(profileService)
        {
            this.blobService = blobService;
            this.searchService = searchService;
        }

        /// <summary>
        /// Index action to return Collection Xml
        /// </summary>
        /// <param name="query">Search string</param>
        /// <returns>Pivot Collection Xml</returns>
        //[ValidateInput(false)]
        //public ActionResult Index(string query)
        //{
        //    var resultsString = !string.IsNullOrWhiteSpace(query) ? string.Format(CultureInfo.CurrentCulture, Properties.Resources.SearchResultForText, query) : Properties.Resources.SearchResultText;
        //    var action = new PivotCollectionActionResult<DeepZoomViewModel>(resultsString);

        //    if (!string.IsNullOrWhiteSpace(query))
        //    {
        //        // TODO : Limit the number of records returned
        //        var results = this.searchService.DeepZoomSearch(query, this.CurrentUserID).ToList();
        //        if (results != null && results.Count > 0)
        //        {
        //            //// Set ActionResult values using Deep Zoom search response Pass dzc Url which will be used as ImageBase for the Pivot collection
        //            action.SetValuesFrom(results, Url.Content("~/DeepZoom/Collection/results.dzc"));
        //            var collectionAction = new DeepZoomCollectionActionResult();

        //            //// Build the DZC collection here itself to keep the mappings of N and ID fields with the result set that is being sent back as part of
        //            //// this request. 
        //            for (int i = 0; i < results.Count; i++)
        //            {
        //                collectionAction.Items.Add(new DeepZoomItem { N = i.ToString(CultureInfo.InvariantCulture), Id = i.ToString(CultureInfo.InvariantCulture), Source = "results/" + results[i].GetDeepZoomItemSource() });
        //            }

        //            // Results need to be added to the session so that they are used in consecutive requests from Pivot Viewer
        //            SessionWrapper.Set<DeepZoomCollectionActionResult>("PivotResults", collectionAction);
        //            return action;
        //        }
        //    }

        //    // Set it to null in case of error
        //    SessionWrapper.Set<DeepZoomCollectionActionResult>("PivotResults", null);
        //    return action;
        //}

        /// <summary>
        /// Returns Deep Zoom Collection result
        /// </summary>
        /// <returns>Deep Zoom Collection Xml</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Need to be accessed by PivotViewer.")]
        public ActionResult Collection()
        {
            return SessionWrapper.Get<DeepZoomCollectionActionResult>("PivotResults");
        }

        /// <summary>
        /// Returns Deep Zoom Item Xml. This is a standard response for all the item requests
        /// </summary>
        /// <returns>Deep Zoom Item Xml</returns>
        public ActionResult Item()
        {
            // TODO : Cache this
            return this.Content(DeepZoomItemXml, "text/xml");
        }

        /// <summary>
        /// This is a dummy call as we don't store a tile pyramid and we don't get the item id in this request
        /// </summary>
        /// <returns>Image tile xml</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Controller action cannot be static")]
        public ActionResult ImageTile()
        {
            // TODO : Cache this
            // This is a dummy call as we don't store a tile pyramid and we don't get the item id in this request
            return null;
        }

        /// <summary>
        /// This is the request that will return the image for the pivot control
        /// </summary>
        /// <returns>File stream result</returns>
        public ActionResult Image(string results, string itemId)
        {
            // TODO : We don't need images for level 1 onwards
            if (!string.IsNullOrEmpty(results))
            {
                Guid itemGuid;

                //// If not GUID, then it is an image string 
                if (Guid.TryParse(itemId, out itemGuid))
                {
                    var blobDetails = this.blobService.GetThumbnail(new Guid(itemId));
                    if (blobDetails != null && blobDetails.Data != null)
                    {
                        return GetFileStream(blobDetails);
                    }
                }
                else
                {
                    return File(Url.Content("~/Content/Images/" + itemId + ".png"), Constants.DefaultMimeType);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the file stream based on the blobDetails
        /// </summary>
        /// <param name="blobDetails">Details of the blob</param>
        /// <returns>File Stream Result </returns>
        private FileStreamResult GetFileStream(BlobDetails blobDetails)
        {
            // Update the response header.
            Response.AddHeader("Content-Encoding", blobDetails.MimeType);

            // Set the position to Begin.
            blobDetails.Data.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(blobDetails.Data, blobDetails.MimeType);
        }
    }
}
