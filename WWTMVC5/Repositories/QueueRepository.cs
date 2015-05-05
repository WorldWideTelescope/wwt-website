//-----------------------------------------------------------------------
// <copyright file="QueueRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the Queue Repository.
    /// </summary>
    public class QueueRepository //: WWTMVC5.Repositories.Interfaces.IQueueRepository TODO: Reimplement interface
        : IQueueRepository
    {
        private static Lazy<CloudBlobClient> lazyBlobClient;
        private static Lazy<CloudQueueClient> lazyQueueClient;
        private Microsoft.WindowsAzure.CloudStorageAccount storageAccount;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueRepository"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Code does not grant its callers access to operations or resources that can be used in a destructive manner.")]
        public QueueRepository()
        {
            storageAccount = CloudStorageAccount.FromConfigurationSetting(Constants.EarthOnlineStorageSettingName);
            lazyBlobClient = new Lazy<CloudBlobClient>(() =>
            {
                var cloudStorageAccount = this.storageAccount;
                return cloudStorageAccount != null ? (cloudStorageAccount.CreateCloudBlobClient()) : null;
            });
            lazyQueueClient = new Lazy<CloudQueueClient>(() => (this.storageAccount.CreateCloudQueueClient()));
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
        /// Gets a handle to the queue directing messages to the Azure services.
        /// </summary>
        public CloudQueue NotificationQueue
        {
            get
            {
                return QueueClient.GetQueueReference(Constants.AzureBackendQueueName);
            }
        }

        /// <summary>
        /// Gets the CloudBlobClient instance from lazyclient.
        /// </summary>
        private static CloudBlobClient BlobClient
        {
            get
            {
                CloudBlobClient blobclient = lazyBlobClient.Value;
                blobclient.RetryPolicy = Constants.DefaultRetryPolicy;
                return blobclient;
            }
        }

        /// <summary>
        /// Gets a CloudQueueClient instance with the preferred retry policy.
        /// </summary>
        private static CloudQueueClient QueueClient
        {
            get
            {
                CloudQueueClient client = lazyQueueClient.Value;
                client.RetryPolicy = Constants.DefaultRetryPolicy;

                return client;
            }
        }

        /// <summary>
        /// Create a queue message from the specified message object.
        /// </summary>
        /// <param name="notification">The notification object to send.</param>
        /// <returns>Azure queue message.</returns>
        public CloudQueueMessage Pack(object notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            // Serialize the message
            byte[] content;
            XmlSerializer serializer = new XmlSerializer(notification.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, notification);
                stream.Flush();
                content = stream.ToArray();
            }

            // Wrap the message in a MessagePayload.
            //
            // An Azure queue message must be less than 8KB in size. In addition, the Azure client
            // library adds overhead in the encoding so roughly 6KB is the limit for the payload.
            // If the content of the initial message is greater than 2KB then we place the content 
            // in Blob storage and include a reference to the Blob in the MessagePayload. Otherwise, 
            // the content of the initial message is embedded directly in the generic message.
            MessagePayload m = new MessagePayload();
            m.NotificationType = notification.GetType().FullName;
            if (content.Length <= Constants.MessageSizeThreshold)
            {
                m.BlobAddress = string.Empty;
                m.EmbeddedContent = content;
            }
            else
            {
                CloudBlobContainer container = GetContainer(Constants.NotificationContainerName);
                string blobAddress = Guid.NewGuid().ToString();
                CloudBlob blob = container.GetBlobReference(blobAddress);
                blob.ServiceClient.ParallelOperationThreadCount = 1;
                blob.UploadByteArray(content);

                m.BlobAddress = blobAddress;
                m.EmbeddedContent = null;
            }

            serializer = new XmlSerializer(typeof(MessagePayload));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, m);
                stream.Flush();
                content = stream.ToArray();
            }

            return new CloudQueueMessage(content);
        }

        /// <summary>
        /// Convert the content of a queue message into a message object.
        /// </summary>
        /// <param name="message">Azure queue message.</param>
        /// <returns>The message object being received.</returns>
        public object Unpack(CloudQueueMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            byte[] content = message.AsBytes;
            MessagePayload genericMessage;
            XmlSerializer serializer = new XmlSerializer(typeof(MessagePayload));
            using (MemoryStream ms = new MemoryStream(content))
            {
                genericMessage = serializer.Deserialize(ms) as MessagePayload;
            }

            content = null;
            if (string.IsNullOrEmpty(genericMessage.BlobAddress))
            {
                content = genericMessage.EmbeddedContent;
            }
            else
            {
                CloudBlobContainer container = GetContainer(Constants.NotificationContainerName);
                string blobAddress = genericMessage.BlobAddress;
                CloudBlob blob = container.GetBlobReference(blobAddress);
                blob.ServiceClient.ParallelOperationThreadCount = 1;
                content = blob.DownloadByteArray();
                blob.Delete();
            }

            object result = null;
            Type targetType = Type.GetType(genericMessage.NotificationType);
            serializer = new XmlSerializer(targetType);
            using (MemoryStream ms = new MemoryStream(content))
            {
                result = serializer.Deserialize(ms);
            }

            return result;
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
            // TODO: Need to make sure the container is created. This was commented out as this was 
            //      proving to be a redundant call to azure and to improve performance
            var blobContainer = new CloudBlobContainer(containerName, BlobClient);
            return blobContainer;
        }
    }
}