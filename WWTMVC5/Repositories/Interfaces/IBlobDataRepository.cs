//-----------------------------------------------------------------------
// <copyright file="IBlobDataRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.WindowsAzure.StorageClient;
using WWTMVC5.Models;

namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the blob data repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IBlobDataRepository
    {
        /// <summary>
        /// Gets the blob content from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        BlobDetails GetBlobContent(string blobName);

        /// <summary>
        /// Gets the thumbnail from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        BlobDetails GetThumbnail(string blobName);

        /// <summary>
        /// Gets the Temporary file(whether thumbnail or file) from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        BlobDetails GetTemporaryFile(string blobName);

        /// <summary>
        /// Uploads a file to azure as a blob.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is uploaded successfully; otherwise false.
        /// </returns>
        bool UploadFile(BlobDetails details);

        /// <summary>
        /// Uploads a thumbnail to azure as a blob.
        /// </summary>
        /// <param name="details">
        /// Details of the thumbnail which has to be uploaded.
        /// </param>
        /// <returns>
        /// True if the thumbnail is uploaded successfully; otherwise false.
        /// </returns>
        bool UploadThumbnail(BlobDetails details);

        /// <summary>
        /// Uploads a file to azure as a blob in temporary container.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is uploaded successfully; otherwise false.
        /// </returns>
        bool UploadTemporaryFile(BlobDetails details);

        /// <summary>
        /// Move a file from temporary container to file container in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is moved successfully; otherwise false.
        /// </returns>
        bool MoveFile(BlobDetails details);
        
        /// <summary>
        /// Move a thumbnail from temporary container to thumbnail container in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the thumbnail which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the thumbnail is moved successfully; otherwise false.
        /// </returns>
        bool MoveThumbnail(BlobDetails details);

        /// <summary>
        /// Deletes a file from azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is deleted successfully; otherwise false.
        /// </returns>
        bool DeleteFile(BlobDetails details);

        /// <summary>
        /// Deletes a thumbnail from azure.
        /// </summary>
        /// <param name="details">
        /// Details of the thumbnail which has to be uploaded.
        /// </param>
        /// <returns>
        /// True if the thumbnail is deleted successfully; otherwise false.
        /// </returns>
        bool DeleteThumbnail(BlobDetails details);

        /// <summary>
        /// Deletes a temporary file from azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is deleted successfully; otherwise false.
        /// </returns>
        bool DeleteTemporaryFile(BlobDetails details);

        /// <summary>
        /// Gets the blob content from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        BlobDetails GetAsset(string blobName);

        /// <summary>
        /// Uploads a file to azure as a blob.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is uploaded successfully; otherwise false.
        /// </returns>
        bool UploadAsset(BlobDetails details);

        /// <summary>
        /// Lists all blobs in asset container
        /// </summary>
        /// <returns>List of blobs</returns>
        IEnumerable<CloudBlob> ListAssets();

        /// <summary>
        /// Deletes a file from azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be deleted.
        /// </param>
        /// <returns>
        /// True if the file is deleted successfully; otherwise false.
        /// </returns>
        bool DeleteAsset(BlobDetails details);

        /// <summary>
        /// Move a file from temporary container to asset container in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is moved successfully; otherwise false.
        /// </returns>
        bool MoveAssetFile(BlobDetails details);

        /// <summary>
        /// Checks a file in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be checked.
        /// </param>
        /// <returns>
        /// True if the file is found successfully; otherwise false.
        /// </returns>
        bool CheckIfAssetExists(BlobDetails details);
    }
}