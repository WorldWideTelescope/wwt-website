//-----------------------------------------------------------------------
// <copyright file="BlobDataRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the Blob Data Repository having methods for adding/retrieving files from
    /// Azure blob storage.
    /// </summary>
    public class BlobDataRepository : IBlobDataRepository
    {
        private static CloudBlobClient _blobClient;
        private CloudStorageAccount _storageAccount;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobDataRepository"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification =
                "Code does not grant its callers access to operations or resources that can be used in a destructive manner."
            )]
        public BlobDataRepository()
        {
            try
            {
                _storageAccount = CloudStorageAccount.Parse(ConfigReader<string>.GetSetting("EarthOnlineStorage"));
                _blobClient = _storageAccount.CreateCloudBlobClient();
            }
            catch (Exception) { }// setting not available
        }

        /// <summary>
        /// Gets the container URL.
        /// </summary>
        public static Uri ContainerUrl
        {
            get
            {
                return new Uri(string.Join(Constants.PathSeparator, new string[] { BlobClient.BaseUri.AbsolutePath, Constants.ContainerName }));
            }
        }

        /// <summary>
        /// Gets the CloudBlobClient instance from lazyclient.
        /// </summary>
        private static CloudBlobClient BlobClient
        {
            get
            {
                return _blobClient;
            }
        }

        /// <summary>
        /// Gets the blob content from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        public BlobDetails GetBlobContent(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentNullException("blobName");
            }

            return GetBlob(blobName, Constants.ContainerName);
        }

        /// <summary>
        /// Gets the thumbnail from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        public BlobDetails GetThumbnail(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentNullException("blobName");
            }

            return GetBlob(blobName, Constants.ThumbnailContainerName);
        }

        /// <summary>
        /// Gets the Temporary file (whether thumbnail or file) from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        public BlobDetails GetTemporaryFile(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentNullException("blobName");
            }

            return GetBlob(blobName, Constants.TemporaryContainerName);
        }

        /// <summary>
        /// Uploads a file to azure as a blob.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is uploaded successfully; otherwise false.
        /// </returns>
        public bool UploadFile(BlobDetails details)
        {
            this.CheckNotNull(() => details);

            return UploadBlobContent(details, Constants.ContainerName);
        }

        /// <summary>
        /// Move a file from temporary container to file container in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is moved successfully; otherwise false.
        /// </returns>
        public bool MoveFile(BlobDetails details)
        {
            return MoveBlob(details, Constants.TemporaryContainerName, Constants.ContainerName);
        }

        /// <summary>
        /// Move a thumbnail from temporary container to thumbnail container in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the thumbnail which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the thumbnail is moved successfully; otherwise false.
        /// </returns>
        public bool MoveThumbnail(BlobDetails details)
        {
            return MoveBlob(details, Constants.TemporaryContainerName, Constants.ThumbnailContainerName);
        }

        /// <summary>
        /// Uploads a thumbnail to azure as a blob.
        /// </summary>
        /// <param name="details">
        /// Details of the thumbnail which has to be uploaded.
        /// </param>
        /// <returns>
        /// True if the thumbnail is uploaded successfully; otherwise false.
        /// </returns>
        public bool UploadThumbnail(BlobDetails details)
        {
            this.CheckNotNull(() => details);

            return UploadBlobContent(details, Constants.ThumbnailContainerName);
        }

        /// <summary>
        /// Uploads a file to azure as a blob in temporary container.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is uploaded successfully; otherwise false.
        /// </returns>
        public bool UploadTemporaryFile(BlobDetails details)
        {
            this.CheckNotNull(() => details);

            return UploadBlobContent(details, Constants.TemporaryContainerName);
        }

        /// <summary>
        /// Deletes a file from azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is deleted successfully; otherwise false.
        /// </returns>
        public bool DeleteFile(BlobDetails details)
        {
            return DeleteBlob(details, Constants.ContainerName);
        }

        /// <summary>
        /// Deletes a thumbnail from azure.
        /// </summary>
        /// <param name="details">
        /// Details of the thumbnail which has to be uploaded.
        /// </param>
        /// <returns>
        /// True if the thumbnail is deleted successfully; otherwise false.
        /// </returns>
        public bool DeleteThumbnail(BlobDetails details)
        {
            return DeleteBlob(details, Constants.ThumbnailContainerName);
        }

        /// <summary>
        /// Deletes a temporary file from azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is deleted successfully; otherwise false.
        /// </returns>
        public bool DeleteTemporaryFile(BlobDetails details)
        {
            return DeleteBlob(details, Constants.TemporaryContainerName);
        }

        /// <summary>
        /// Gets the blob content from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        public BlobDetails GetAsset(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentNullException("blobName");
            }

            return GetBlob(blobName, Constants.AssetContainerName);
        }

        /// <summary>
        /// Uploads a file to azure as a blob.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is uploaded successfully; otherwise false.
        /// </returns>
        public bool UploadAsset(BlobDetails details)
        {
            this.CheckNotNull(() => details);
            
            return UploadBlobContent(details, Constants.AssetContainerName);
        }

        

        /// <summary>
        /// Deletes a file from azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be deleted.
        /// </param>
        /// <returns>
        /// True if the file is deleted successfully; otherwise false.
        /// </returns>
        public bool DeleteAsset(BlobDetails details)
        {
            return DeleteBlob(details, Constants.AssetContainerName);
        }

        /// <summary>
        /// Move a file from temporary container to asset container in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be uploaded to azure.
        /// </param>
        /// <returns>
        /// True if the file is moved successfully; otherwise false.
        /// </returns>
        public bool MoveAssetFile(BlobDetails details)
        {
            return MoveBlob(details, Constants.TemporaryContainerName, Constants.AssetContainerName);
        }

        /// <summary>
        /// Checks a file in azure.
        /// </summary>
        /// <param name="details">
        /// Details of the file which has to be checked.
        /// </param>
        /// <returns>
        /// True if the file is found successfully; otherwise false.
        /// </returns>
        public bool CheckIfAssetExists(BlobDetails details)
        {
            return ExistsBlobContent(details, Constants.AssetContainerName);
        }

        /// <summary>
        /// Gets the content from azure as a stream.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <param name="outputStream">
        /// The content is exposed as output stream.
        /// </param>
        /// <param name="container">
        /// COntainer where we could find the blob.
        /// </param>
        /// <returns>
        /// The blob properties.
        /// </returns>
        private static BlobProperties GetContent(string blobName, Stream outputStream, CloudBlobContainer container)
        {
            var blob = container.GetBlobReferenceFromServer(blobName.ToUpperInvariant());
            blob.DownloadToStream(outputStream);
            return blob.Properties;
        }

        /// <summary>
        /// Used to retrieve the container reference identified by the container name.
        /// </summary>
        /// <param name="containerName">
        /// Name of the container.
        /// </param>
        /// <returns>
        /// Container instance.
        /// </returns>
        private static CloudBlobContainer GetContainer(string containerName)
        {
            var container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
            return container;
        }

        /// <summary>
        /// Gets the content of the blob from specified container.
        /// </summary>
        /// <param name="blobName">
        /// Name of the blob.
        /// </param>
        /// <param name="containerName">
        /// name of the container.
        /// </param>
        /// <returns>
        /// The blob details.
        /// </returns>
        private static BlobDetails GetBlob(string blobName, string containerName)
        {
            Stream outputStream = null;
            BlobProperties properties = null;
            try
            {
                outputStream = new MemoryStream();
                var container = GetContainer(containerName);
                properties = GetContent(blobName, outputStream, container);
            }
            catch (InvalidOperationException)
            {
                // TODO: Add error handling logic
                // "Error getting contents of blob {0}: {1}", ContainerUrl + _PathSeparator + blobName, sc.Message
                outputStream = null;
            }
            catch (StorageException)
            {
                
                // TODO: Add error handling logic
                // ErrorCode and StatusCode can be used to identify the error.
                // "Error getting contents of blob {0}: {1}", ContainerUrl + _PathSeparator + blobName, sc.Message

                // TODO: Need to add proper Exception handling.
                outputStream = null;
            }

            return new BlobDetails()
            {
                Data = outputStream,
                BlobID = blobName,
                MimeType = properties != null ? properties.ContentType : Constants.DefaultMimeType
            };
        }

        /// <summary>
        /// Moves blob from source to destination.
        /// </summary>
        /// <param name="details">
        /// Details of the blob.
        /// </param>
        /// <param name="sourceContainerName">
        /// Name of Source container.
        /// </param>
        /// <param name="destinationContainerName">
        /// Name of Destination container.
        /// </param>
        /// <returns>
        /// True, if the blob is successfully moved;otherwise false.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ignore all exceptions.")]
        private static bool MoveBlob(BlobDetails details, string sourceContainerName, string destinationContainerName)
        {
            try
            {
                var sourceContainer = GetContainer(sourceContainerName);
                var destinationContainer = GetContainer(destinationContainerName);

                // TODO: Check if the input file type and then use either block blob or page blob.
                // For plate file we need to upload the file as page blob.
                var sourceBlob = sourceContainer.GetBlockBlobReference(details.BlobID.ToUpperInvariant());
                var destinationBlob = destinationContainer.GetBlockBlobReference(details.BlobID.ToUpperInvariant());

                destinationBlob.StartCopyFromBlob(sourceBlob);
                destinationBlob.Properties.ContentType = sourceBlob.Properties.ContentType;
                sourceBlob.Delete();
                return true;
            }
            catch (Exception)
            {
                // "Error moving blob {0}: {1}", ContainerUrl + _PathSeparator + blobName, se.Message
            }

            return false;
        }

        /// <summary>
        /// Checks if the blob content is present in azure or not.
        /// </summary>
        /// <param name="details">
        /// Details of the blob.
        /// </param>
        /// <param name="containerName">
        /// Name of the container.
        /// </param>
        /// <returns>
        /// True, if the blob is successfully found to azure;otherwise false.
        /// </returns>
        private static bool ExistsBlobContent(BlobDetails details, string containerName)
        {
            try
            {
                var container = GetContainer(containerName);

                // TODO: Check if the input file type and then use either block blob or page blob.
                // For plate file we need to upload the file as page blob.
                var blob = container.GetBlobReferenceFromServer(details.BlobID.ToUpperInvariant());
                blob.FetchAttributes();

                return true;
            }
            catch (StorageException)
            {
                // "Error uploading blob {0}: {1}", ContainerUrl + _PathSeparator + blobName, se.Message
            }

            return false;
        }

        /// <summary>
        /// Deletes the specified file pointed by the blob.
        /// </summary>
        /// <param name="details">
        /// Details of the blob.
        /// </param>
        /// <param name="containerName">
        /// Name of the container.
        /// </param>
        private static bool DeleteBlob(BlobDetails details, string containerName)
        {
            try
            {
                var container = GetContainer(containerName);

                container.GetBlobReferenceFromServer(details.BlobID.ToUpperInvariant()).Delete();
                return true;
            }
            catch (InvalidOperationException)
            {
                // "Error deleting blob {0}: {1}", ContainerUrl + _PathSeparator + blobName, se.Message
                return false;
            }
        }

        /// <summary>
        /// Upload the blob content to azure.
        /// </summary>
        /// <param name="details">
        /// Details of the blob.
        /// </param>
        /// <param name="containerName">
        /// Name of the container.
        /// </param>
        /// <returns>
        /// True, if the blob is successfully uploaded to azure;otherwise false.
        /// </returns>
        private bool UploadBlobContent(BlobDetails details, string containerName)
        {
            this.CheckNotNull(() => details);

            try
            {
                var container = GetContainer(containerName);

                // Seek to start.
                details.Data.Position = 0;

                // TODO: Check if the input file type and then use either block blob or page blob.
                // For plate file we need to upload the file as page blob.
                var blob = container.GetBlockBlobReference(details.BlobID.ToUpperInvariant());
                blob.Properties.ContentType = details.MimeType;
                blob.UploadFromStream(details.Data);

                return true;
            }
            catch (InvalidOperationException)
            {
                // "Error uploading blob {0}: {1}", ContainerUrl + _PathSeparator + blobName, se.Message
            }

            return false;
        }
    }
}