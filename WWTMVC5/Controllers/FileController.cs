//-----------------------------------------------------------------------
// <copyright file="FileController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Web.Mvc;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Controllers
{
    /// <summary>
    /// Controller for handling the downloading the thumbnail, files and background images.
    /// </summary>
    public class FileController : Controller
    {
        private IBlobService _blobService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// </summary>
        /// <param name="blobService">Instance of a blob service</param>
        public FileController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        /// <summary>
        /// This is used to retrieve the thumbnail from azure.
        /// </summary>
        /// <param name="id">
        /// ID of thumbnail which has to be retrieved.
        /// </param>
        /// <param name="fullsize"></param>
        /// <returns>
        /// Returns the response as a image.
        /// </returns>
        [HttpGet]
        [Route("File/Thumbnail/{id}/{fullsize=false}")]
        public ActionResult Thumbnail(Guid? id, bool fullsize)
        {
            if (id.HasValue)
            {
                // Get the thumbnail from Azure.
                var blobDetails = _blobService.GetThumbnail(id.Value);

                if (blobDetails != null && blobDetails.Data != null)
                {
                    if (!fullsize)
                    {
                        // Update the size of the thumbnail to 160 X 96 and upload it to temporary container in Azure.
                        blobDetails.Data = blobDetails.Data.GenerateThumbnail(Constants.DefaultClientThumbnailWidth,
                            Constants.DefaultClientThumbnailHeight, Constants.DefaultThumbnailImageFormat);
                    }
                    blobDetails.MimeType = Constants.DefaultThumbnailMimeType;

                    return GetFileStream(blobDetails);
                }
            }

            return null;
        }

        /// <summary>
        /// This is used to retrieve the temporary file(whether thumbnail/file etc.) from azure.
        /// </summary>
        /// <param name="id">
        /// ID of thumbnail which has to be retrieved.
        /// </param>
        /// <returns>
        /// Returns the response as a image.
        /// </returns>
        [HttpGet]
        [Route("File/TemporaryFile/{id}")]
        public ActionResult TemporaryFile(Guid? id)
        {
            if (id.HasValue)
            {
                // Get the thumbnail from Azure.
                var blobDetails = _blobService.GetTemporaryFile(id.Value);

                if (blobDetails != null && blobDetails.Data != null)
                {
                    return GetFileStream(blobDetails);
                }
            }

            return null;
        }

        /// <summary>
        /// This is used to retrieve the thumbnail from azure for WWT client.
        /// The size of the output thumbnail would be 96 X 45
        /// </summary>
        /// <param name="id">
        /// ID of thumbnail which has to be retrieved.
        /// </param>
        /// <returns>
        /// Returns the response as a image.
        /// </returns>
        [HttpGet]
        public ActionResult ClientThumbnail(Guid? id)
        {
            if (id.HasValue)
            {
                // Get the thumbnail from Azure.
                var blobDetails = _blobService.GetThumbnail(id.Value);

                if (blobDetails != null && blobDetails.Data != null)
                {
                    // Update the size of the thumbnail to 160 X 96 and upload it to temporary container in Azure.
                    blobDetails.Data = blobDetails.Data.GenerateThumbnail(Constants.DefaultClientThumbnailWidth, Constants.DefaultClientThumbnailHeight, Constants.DefaultThumbnailImageFormat);
                    blobDetails.MimeType = Constants.DefaultThumbnailMimeType;

                    return GetFileStream(blobDetails);
                }
            }

            return null;
        }

        /// <summary>
        /// This is used to retrieve the content files from azure.
        /// </summary>
        /// <param name="id">
        /// ID of file which has to be retrieved.
        /// </param>
        /// <param name="name">
        /// Name of the file to be downloaded
        /// </param>
        /// <param name="ext"></param>
        /// <returns>
        /// Returns the response as a file stream.
        /// </returns>
        [HttpGet]
        [Route("File/Download/{id}/{name}/{ext=none}")]
        public ActionResult Download(Guid? id, string name, string ext)
        {
            if (id.HasValue)
            {
                // Get the file from Azure.
                var blobDetails = _blobService.GetFile(id.Value);

                if (blobDetails != null && blobDetails.Data != null)
                {
                    // Update the response header.
                    Response.AddHeader("Content-Encoding", blobDetails.MimeType);
                    var filename = ext == "none" ? name : name + "." + ext;
                    Response.AddHeader("content-disposition", "attachment;filename=" + Uri.UnescapeDataString(filename));

                    // Set the position to Begin.
                    blobDetails.Data.Seek(0, SeekOrigin.Begin);
                    
                    return new FileStreamResult(blobDetails.Data, blobDetails.MimeType);
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