//-----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Mvc;
using Microsoft.WindowsAzure.StorageClient;
using WWTMVC5.Repositories.Interfaces;

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Class having the extension methods needed for Object class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Adds the object to the notification queue.
        /// </summary>
        /// <param name="request">Request to be added to notification queue.</param>
        public static void AddToNotificationQueue(this object request)
        {
            IQueueRepository queueRepository = DependencyResolver.Current.GetService(typeof(IQueueRepository)) as IQueueRepository;
            CloudQueueMessage message = queueRepository.Pack(request);
            queueRepository.NotificationQueue.AddMessage(message);
        }
    }
}