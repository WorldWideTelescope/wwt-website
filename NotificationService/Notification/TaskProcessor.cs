//-----------------------------------------------------------------------
// <copyright file="TaskProcessor.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Implements a means to sequentially process a stream of incoming tasks on a dedicated thread.
    /// </summary>
    public class TaskProcessor
    {
        /// <summary>
        /// Initializes a new instance of the TaskProcessor class.
        /// </summary>
        /// <param name="nextTask">A function which provides incoming tasks one a a time.</param>
        /// <param name="resultQueue">The concurrent queue in which results produced by the tasks are added.</param>
        public TaskProcessor(Func<ITask> nextTask, ConcurrentQueue<TaskResult> resultQueue)
        {
            this.NextTask = nextTask;
            this.ResultQueue = resultQueue;
            this.Pause = 1000;
            this.IsStopped = false;
            this.IsCancelled = false;
        }

        /// <summary>
        /// Gets the function which provides incoming tasks.
        /// </summary>
        public Func<ITask> NextTask { get; private set; }

        /// <summary>
        /// Gets the queue in which computed results are made available.
        /// </summary>
        public ConcurrentQueue<TaskResult> ResultQueue { get; private set; }

        /// <summary>
        /// Gets or sets the duration (in milliseconds) for which the processor should sleep
        /// after checking for the presence of a task and after finding out that there is no
        /// work to perform.
        /// </summary>
        public int Pause { get; set; }

        /// <summary>
        /// Gets a value indicating whether the processing loop has been cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the processing loop is stopped.
        /// </summary>
        public bool IsStopped { get; private set; }

        /// <summary>
        /// Gets the task which is currently being processed.
        /// </summary>
        public ITask ActiveTask { get; private set; }

        /// <summary>
        /// Gets or sets the thread on which the processing is happening.
        /// </summary>
        private Thread RunningThread { get; set; }

        /// <summary>
        /// Start processing the tasks on a new thread.
        /// </summary>
        public void Start()
        {
            ThreadStart threadDelegate = new ThreadStart(this.Run);
            this.RunningThread = new Thread(threadDelegate);
            this.RunningThread.Start();
        }

        /// <summary>
        /// Stop processing the tasks.
        /// </summary>
        /// <remarks>
        /// Processing may not stop immediately if work is going on. Check the value of the IsStopped
        /// property to find out if the processing has in fact stopped.
        /// </remarks>
        public void Stop()
        {
            this.IsCancelled = true;
        }

        /// <summary>
        /// Implements a loop to process the stream of tasks.
        /// </summary>
        private void Run()
        {
            while (this.IsCancelled == false)
            {
                this.ActiveTask = this.NextTask();
                if (this.ActiveTask == null)
                {
                    System.Threading.Thread.Sleep(this.Pause);
                }
                else
                {
                    try
                    {
                        this.ActiveTask.Execute(this);
                    }
                    catch (Exception ex)
                    {
                        Logger.Logger.Error(ex, string.Format("Execution of a task produced an uncaught exception."));
                    }
                }
            }

            this.IsStopped = true;
        }
    }
}
