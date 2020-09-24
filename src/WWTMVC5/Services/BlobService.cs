//-----------------------------------------------------------------------
// <copyright file="BlobService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Properties;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5.Services
{
    public class BlobService : IBlobService
    {
        private IBlobDataRepository _blobDataRepository;
        
        /// <summary>
        /// Initializes a new instance of the BlobService class.
        /// </summary>
        /// <param name="blobDataRepository">
        /// Instance of blob data repository.
        /// </param>
        public BlobService(IBlobDataRepository blobDataRepository)
        {
            this._blobDataRepository = blobDataRepository;
        }

        /// <summary>
        /// Gets the files from azure which is identified by file id.
        /// </summary>
        /// <param name="fileId">
        /// ID of the file.
        /// </param>
        /// <returns>
        /// Blob details.
        /// </returns>
        public BlobDetails GetFile(Guid fileId)
        {
            return _blobDataRepository.GetBlobContent(fileId.ToString().ToUpperInvariant());
        }

        /// <summary>
        /// Gets the files from azure which is identified by thumbnail id.
        /// </summary>
        /// <param name="thumbnailId">
        /// ID of the thumbnail.
        /// </param>
        /// <returns>
        /// Blob details.
        /// </returns>
        public BlobDetails GetThumbnail(Guid thumbnailId)
        {
            return _blobDataRepository.GetThumbnail(thumbnailId.ToString().ToUpperInvariant());
        }

        /// <summary>
        /// Gets the Temporary file(whether thumbnail/video from azure which is identified by thumbnail id.
        /// </summary>
        /// <param name="thumbnailId">
        /// ID of the thumbnail.
        /// </param>
        /// <returns>
        /// Blob Details.
        /// </returns>
        public BlobDetails GetTemporaryFile(Guid thumbnailId)
        {
            return _blobDataRepository.GetTemporaryFile(thumbnailId.ToString().ToUpperInvariant());
        }

        /// <summary>
        /// Move Home video from temp to correct container.
        /// </summary>
        /// <param name="fileDetails">Details of the video.</param>
        public OperationStatus MoveTempFile(FileDetail fileDetails)
        {
            OperationStatus operationStatus = null;

            this.CheckNotNull(() => new { fileDetails });

            // Move Home video.
            if (fileDetails.AzureID != null)
            {
                // Move the video file from temporary container to file container.
                try
                {
                    if (MoveAssetFile(fileDetails))
                    {
                        operationStatus = OperationStatus.CreateSuccessStatus();
                    }
                    else
                    {
                        operationStatus = OperationStatus.CreateFailureStatus(Resources.UnknownErrorMessage);
                    }
                }
                catch (Exception)
                {
                    operationStatus = OperationStatus.CreateFailureStatus(Resources.UnknownErrorMessage);
                }
            }
            return operationStatus;
        }

        /// <summary>
        /// Gets the files from azure which is identified by file name.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <returns>
        /// Blob details.
        /// </returns>
        public BlobDetails GetAsset(string fileName)
        {
            return _blobDataRepository.GetAsset(fileName);
        }

        /// <summary>
        /// Uploads file to azure.
        /// </summary>
        /// <param name="fileDetails">Details of the file.</param>
        public OperationStatus UploadAsset(FileDetail fileDetails)
        {
            OperationStatus operationStatus = null;
            this.CheckNotNull(() => new { fileDetails });

            var fileBlob = new BlobDetails()
            {
                BlobID = fileDetails.Name,
                Data = fileDetails.DataStream,
                MimeType = fileDetails.MimeType
            };
            try
            {
                if (_blobDataRepository.UploadAsset(fileBlob))
                {
                    operationStatus = OperationStatus.CreateSuccessStatus();
                }
                else
                {
                    operationStatus = OperationStatus.CreateFailureStatus(Resources.UnknownErrorMessage);
                }
            }
            catch (Exception)
            {
                operationStatus = OperationStatus.CreateFailureStatus(Resources.UnknownErrorMessage);
            }

            return operationStatus;
        }

        

        /// <summary>
        /// Deletes the file from azure which is identified by file name.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <returns>
        /// Operation Status
        /// </returns>
        public OperationStatus DeleteAsset(string fileName)
        {
            OperationStatus operationStatus = null;
            var fileBlob = new BlobDetails()
            {
                BlobID = fileName,
            };

            try
            {
                _blobDataRepository.DeleteAsset(fileBlob);
                operationStatus = OperationStatus.CreateSuccessStatus();
            }
            catch (Exception)
            {
                operationStatus = OperationStatus.CreateFailureStatus(Resources.UnknownErrorMessage);
            }

            return operationStatus;
        }

        /// <summary>
        /// Checks the file from azure which is identified by file name.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <returns>
        /// Operation Status
        /// </returns>
        public OperationStatus CheckIfAssetExists(string fileName)
        {
            OperationStatus operationStatus = null;
            var fileBlob = new BlobDetails()
            {
                BlobID = fileName,
            };

            try
            {
                if (_blobDataRepository.CheckIfAssetExists(fileBlob))
                {
                    operationStatus = OperationStatus.CreateSuccessStatus();
                }
                else
                {
                    operationStatus = OperationStatus.CreateFailureStatus(Resources.UnknownErrorMessage);
                }
            }
            catch (Exception ex)
            {
                operationStatus = OperationStatus.CreateFailureStatus(Resources.UnknownErrorMessage, ex);
            }

            return operationStatus;
        }

        /// <summary>
        /// Uploads the associated file to temporary container.
        /// </summary>
        /// <param name="fileDetail">Details of the associated file.</param>
        /// <returns>True if content is uploaded; otherwise false.</returns>
        public OperationStatus UploadTemporaryFile(FileDetail fileDetail)
        {
            OperationStatus operationStatus = null;

            // Make sure file detail is not null
            this.CheckNotNull(() => new { fileDetail });

            var fileBlob = new BlobDetails()
            {
                BlobID = fileDetail.AzureID.ToString(),
                Data = fileDetail.DataStream,
                MimeType = fileDetail.MimeType
            };

            try
            {
                _blobDataRepository.UploadTemporaryFile(fileBlob);
                operationStatus = OperationStatus.CreateSuccessStatus();
            }
            catch (Exception)
            {
                operationStatus = OperationStatus.CreateFailureStatus(Resources.UnknownErrorMessage);
            }

            return operationStatus;
        }

        /// <summary>
        /// Move temporary file to actual container in azure.
        /// </summary>
        /// <param name="fileDetails">Details of the file.</param>
        private bool MoveAssetFile(FileDetail fileDetails)
        {
            var fileBlob = new BlobDetails()
            {
                BlobID = fileDetails.AzureID.ToString(),
                MimeType = fileDetails.MimeType
            };

            return _blobDataRepository.MoveAssetFile(fileBlob);
        }
    }
}