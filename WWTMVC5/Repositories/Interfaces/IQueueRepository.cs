//-----------------------------------------------------------------------
// <copyright file="IQueueRepository.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


using Microsoft.WindowsAzure.StorageClient;
namespace WWTMVC5.Repositories.Interfaces
{
    /// <summary>
    /// Interface representing the azure queue repository methods. Also, needed for adding unit test cases.
    /// </summary>
    public interface IQueueRepository
    {
        /// <summary>
        /// Gets a handle to the queue directing messages to the Azure services.
        /// </summary>
        CloudQueue NotificationQueue { get; }

        /// <summary>
        /// Create a queue message from the specified message object.
        /// </summary>
        /// <param name="notification">The notification object to send.</param>
        /// <returns>Azure queue message.</returns>
        CloudQueueMessage Pack(object notification);

        /// <summary>
        /// Convert the content of a queue message into a message object.
        /// </summary>
        /// <param name="message">Azure queue message.</param>
        /// <returns>The message object being received.</returns>
        object Unpack(CloudQueueMessage message);
    }
}