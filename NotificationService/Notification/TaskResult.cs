//-----------------------------------------------------------------------
// <copyright file="TaskResult.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Defines a result from a task.
    /// </summary>
    public class TaskResult
    {
        /// <summary>
        /// Initializes a new instance of the TaskResult class.
        /// </summary>
        /// <param name="task">A reference to the task object that produced the result.</param>
        /// <param name="value">The value of the result.</param>
        public TaskResult(ITask task, object value)
        {
            this.Task = task;
            this.Value = value;
        }

        /// <summary>
        /// Gets a reference to the task that produced this result.
        /// </summary>
        public ITask Task { get; private set; }

        /// <summary>
        /// Gets the value of the result.
        /// </summary>
        public object Value { get; private set; }
    }
}
