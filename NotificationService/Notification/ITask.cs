//-----------------------------------------------------------------------
// <copyright file="ITask.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Defines a task to be carried by the notification engine.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Executes the tasks in the given context.
        /// </summary>
        void Execute(TaskProcessor context);
    }
}
