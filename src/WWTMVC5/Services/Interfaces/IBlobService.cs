//-----------------------------------------------------------------------
// <copyright file="IBlobService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.Services.Interfaces
{
    /// <summary>
    /// Interface representing the blob service methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// Gets the files from azure which is identified by file id.
        /// </summary>
        /// <param name="fileId">
        /// ID of the file.
        /// </param>
        /// <returns>
        /// Blob Details.
        /// </returns>
        BlobDetails GetFile(Guid fileId);

        /// <summary>
        /// Gets the files from azure which is identified by thumbnail id.
        /// </summary>
        /// <param name="thumbnailId">
        /// ID of the thumbnail.
        /// </param>
        /// <returns>
        /// Blob Details.
        /// </returns>
        BlobDetails GetThumbnail(Guid thumbnailId);

        /// <summary>
        /// Gets the Temporary file (whether thumbnail/video etc.) from azure which is identified by thumbnail id.
        /// </summary>
        /// <param name="thumbnailId">
        /// ID of the thumbnail.
        /// </param>
        /// <returns>
        /// Blob Details.
        /// </returns>
        BlobDetails GetTemporaryFile(Guid thumbnailId);

        /// <summary>
        /// Gets the files from azure which is identified by file name.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <returns>
        /// Blob Details.
        /// </returns>
        BlobDetails GetAsset(string fileName);

        /// <summary>
        /// Uploads file to azure.
        /// </summary>
        /// <param name="fileDetails">Details of the file.</param>
        OperationStatus UploadAsset(FileDetail fileDetails);

        ///// <summary>
        ///// Lists all blobs in asset container
        ///// </summary>
        ///// <returns>List of blobs</returns>
        //IEnumerable<BlobDetails> ListAssets(PageDetails pageDetails);

        /// <summary>
        /// Uploads the associated file to temporary container.
        /// </summary>
        /// <param name="fileDetail">Details of the associated file.</param>
        /// <returns>True if content is uploaded; otherwise false.</returns>
        OperationStatus UploadTemporaryFile(FileDetail fileDetail);

                /// <summary>
        /// Move Home video from temp to correct container.
        /// </summary>
        /// <param name="fileDetails">Details of the video.</param>
        OperationStatus MoveTempFile(FileDetail fileDetails);

        /// <summary>
        /// Deletes the file from azure which is identified by file name.
        /// </summary>
        /// <param name="fileName">
        /// file name.
        /// </param>
        /// <returns>
        /// Operation Status
        /// </returns>
        OperationStatus DeleteAsset(string fileName);

        /// <summary>
        /// Checks a file in azure.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file which has to be checked.
        /// </param>
        /// <returns>
        /// True if the file is found successfully; otherwise false.
        /// </returns>
        OperationStatus CheckIfAssetExists(string fileName);
    }
}
