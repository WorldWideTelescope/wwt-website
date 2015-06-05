//-----------------------------------------------------------------------
// <copyright file="AssetController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Services.Interfaces;
using WWTMVC5.ViewModels;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the static assets like images.
    /// </summary>
    public class AssetController : ControllerBase
    {
        private IBlobService _blobService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetController"/> class.
        /// </summary>
        /// <param name="blobService">Instance of a blob service</param>
        /// <param name="profileService">Instance of profile Service</param>
        public AssetController(IBlobService blobService, IProfileService profileService)
            : base(profileService)
        {
            this._blobService = blobService;
        }

        /// <summary>
        /// This is used to retrieve the assets from azure container.
        /// </summary>
        /// <param name="id">
        /// Name of the file to be downloaded
        /// </param>
        /// <returns>
        /// Returns the response as a file stream.
        /// </returns>
        [HttpGet]
        public ActionResult Download(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                // Get the file from Azure.
                var blobDetails = this._blobService.GetAsset(id);

                if (blobDetails != null && blobDetails.Data != null)
                {
                    return GetFileStream(blobDetails);
                }
            }

            return null;
        }

        /// <summary>
        /// Controller action which gets the asset upload view.
        /// </summary>
        //[HttpGet]
        //
        //public ActionResult Upload()
        //{
        //    CheckIfSiteAdmin(); 
        //    var uploadAssetViewModel = GetAssets(1);
        //    return System.Web.UI.WebControls.View("UploadAssetView", uploadAssetViewModel);
        //}

        /// <summary>
        /// Controller action which uploads asset.
        /// </summary>
        /// <param name="assetFile">HttpPostedFileBase instance</param>
        /*[HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase assetFile)
        {
            CheckIfSiteAdmin();

            if (assetFile != null)
            {
                // Get File details.
                var fileDetail = new FileDetail();
                fileDetail.SetValuesFrom(assetFile);

                // Check whether file is already available in the azure.
                var operationStatus = this.blobService.CheckIfAssetExists(assetFile.FileName.ToUpper(CultureInfo.CurrentCulture));
                if (!operationStatus.Succeeded)
                {
                    this.blobService.UploadAsset(fileDetail);
                    ViewData["DuplicateFileError"] = string.Empty;
                }
                else
                {
                    ViewData["DuplicateFileError"] = Resources.DuplicateAssetFileErrorMessage;
                }
            }

            var uploadAssetViewModel = GetAssets(1);
            return System.Web.UI.WebControls.View("UploadAssetView", uploadAssetViewModel);
        }*/

        /// <summary>
        /// It returns the assets list view
        /// </summary>
        /// <param name="currentPage">Current page to be rendered</param>
        [HttpPost]

        public void AjaxRenderAssets(int currentPage)
        {
            CheckIfSiteAdmin();

            try
            {
                PartialView("AssetListView", GetAssets(currentPage)).ExecuteResult(this.ControllerContext);
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
        }

        /// <summary>
        /// Removes the selected asset.
        /// Return success / failure based on service method call.
        /// </summary>
        /// <param name="assetId">Asset id which is being deleted</param>
        /// <param name="currentPage">Current page from where action performed</param>
        /// <returns>Ajax response string</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string DeleteAssetRequest(string assetId, int currentPage)
        {
            CheckIfSiteAdmin();

            string ajaxResponse = string.Empty;

            var operationStatus = this._blobService.DeleteAsset(assetId);

            if (operationStatus.Succeeded)
            {
                PartialView("AssetListView", GetAssets(currentPage)).ExecuteResult(this.ControllerContext);
            }
            else if (operationStatus.CustomErrorMessage)
            {
                ajaxResponse = operationStatus.ErrorMessage;
            }

            return ajaxResponse;
        }

        /// <summary>
        /// Controller action which gets the upload home video view.
        /// </summary>
        /*[HttpGet]
        
        public ActionResult UploadHomeVideo()
        {
            CheckIfSiteAdmin();
            VideoDataViewModel videoDataViewModel = new VideoDataViewModel();
            return View("UploadHomeVideoView", videoDataViewModel);
        }*/

        /// <summary>
        /// Controller action which moves the uploaded home video from temp to asset container.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadHomeVideo(VideoDataViewModel uploadedHomeVideo)
        {
            this.CheckNotNull(() => new { uploadedHomeVideo });

            CheckIfSiteAdmin();
            if (uploadedHomeVideo.VideoID != Guid.Empty)
            {
                // Application variable for maintaining version of Home video
                 HttpContext.Application["HomeVideoEtag"] = DateTime.Now;

                // Get File details.
                var fileDetail = new FileDetail();
                fileDetail.AzureID = uploadedHomeVideo.VideoID;
                this._blobService.MoveTempFile(fileDetail);
            }
            return RedirectToAction("index", "Home");
        }

        /// <summary>
        /// Controller action which uploads the video in temp container.
        /// </summary>
        /// <param name="assetFile">HttpPostedFileBase instance</param>
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult AddVideo(HttpPostedFileBase assetFile)
        {
            VideoDataViewModel videoDataViewModel = new VideoDataViewModel();
            try
            {
                if (assetFile != null)
                {
                    // Get File details.
                    var fileDetail = new FileDetail();
                    fileDetail.SetValuesFrom(assetFile);

                    fileDetail.AzureID = videoDataViewModel.VideoID = new Guid(Constants.HomePageVideoGuid);
                    videoDataViewModel.VideoName = Path.GetFileName(assetFile.FileName);

                    // Upload video file in the temporary container. Once the user publishes the content 
                    // then we will move the file from temporary container to the actual container.
                    this._blobService.UploadTemporaryFile(fileDetail);
                }
            }
            catch (Exception)
            {
                // Consume the exception and render rest of the views in the page.
                // TODO: Log the exception?
            }
            return View("UploadHomeVideoView", videoDataViewModel);
        }

        /// <summary>
        /// Gets all the Assets
        /// </summary>
        /// <param name="currentPage">Current page to be rendered</param>
        /// <returns>ViewModel with asset details</returns>
        private UploadAssetViewModel GetAssets(int currentPage)
        {
            PageDetails pageDetails = new PageDetails(currentPage);
            pageDetails.ItemsPerPage = Constants.PermissionsPerPage;

            var blobDetails = this._blobService.ListAssets(pageDetails);
            this.CheckNotNull(() => new { blobDetails });

            // Remove the detail of home page video from the blobDetails
            var details = blobDetails.Where(item => item.BlobID.ToUpperInvariant() != Constants.HomePageVideoGuid.ToUpperInvariant()).ToList();
            UploadAssetViewModel uploadAssetViewModel = new UploadAssetViewModel(details, pageDetails);

            return uploadAssetViewModel;
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