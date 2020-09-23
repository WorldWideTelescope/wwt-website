//-----------------------------------------------------------------------
// <copyright file="QueueRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace WWTMVC5.Repositories
{
    /// <summary>
    /// Class representing the Queue Repository.
    /// </summary>
    public class QueueRepository : IQueueRepository
    {
        
        private static CloudQueueClient _queueClient;
        private static CloudBlobClient _blobClient;
        private CloudStorageAccount _storageAccount;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueRepository"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Code does not grant its callers access to operations or resources that can be used in a destructive manner.")]
        public QueueRepository()
        {
            try
            {
                _storageAccount = CloudStorageAccount.Parse(ConfigReader<string>.GetSetting("EarthOnlineStorage"));
                _blobClient = _storageAccount.CreateCloudBlobClient();
                _queueClient = _storageAccount.CreateCloudQueueClient();
            }
            catch (Exception) { }//fail silently for developers without valid storage settings
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
                
                return _blobClient;
            }
        }

        /// <summary>
        /// Gets a CloudQueueClient instance with the preferred retry policy.
        /// </summary>
        private static CloudQueueClient QueueClient
        {
            get { return _queueClient; }
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
            var serializer = new XmlSerializer(notification.GetType());
            using (var stream = new MemoryStream())
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
            var m = new MessagePayload();
            m.NotificationType = notification.GetType().FullName;
            if (content.Length <= Constants.MessageSizeThreshold)
            {
                m.BlobAddress = string.Empty;
                m.EmbeddedContent = content;
            }
            else
            {
                var container = GetContainer(Constants.NotificationContainerName);
                var blobAddress = Guid.NewGuid().ToString();
                var blob = container.GetBlobReferenceFromServer(blobAddress);
                blob.ServiceClient.DefaultRequestOptions.ParallelOperationThreadCount = 1;
                blob.UploadFromByteArray(content,0,content.Length);

                m.BlobAddress = blobAddress;
                m.EmbeddedContent = null;
            }

            serializer = new XmlSerializer(typeof(MessagePayload));
            using (var stream = new MemoryStream())
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

            var content = message.AsBytes;
            MessagePayload genericMessage;
            var serializer = new XmlSerializer(typeof(MessagePayload));
            using (var ms = new MemoryStream(content))
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
                var container = GetContainer(Constants.NotificationContainerName);
                var blobAddress = genericMessage.BlobAddress;
                var blob = container.GetBlobReferenceFromServer(blobAddress);
                blob.ServiceClient.DefaultRequestOptions.ParallelOperationThreadCount = 1;
                blob.DownloadToByteArray(content, 0);
                blob.Delete();
            }

            object result = null;
            var targetType = Type.GetType(genericMessage.NotificationType);
            serializer = new XmlSerializer(targetType);
            using (var ms = new MemoryStream(content))
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
            var container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
            return container;
        }
    }
}