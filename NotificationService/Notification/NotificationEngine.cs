//-----------------------------------------------------------------------
// <copyright file="NotificationEngine.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using WWTMVC5;
using WWTMVC5.Models;
using WWTMVC5.Repositories;
using WWTMVC5.Repositories.Interfaces;

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Class implements core functionalities of Notification service
    /// </summary>
    public class NotificationEngine
    {
        private static UnityContainer container;

        private AutoResetEvent stop;
        private AutoResetEvent pause;
        private TaskProcessor notificationTasksProc;
        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the NotificationEngine class.
        /// </summary>
        public NotificationEngine()
        {
            this.stop = new AutoResetEvent(false);
            this.pause = new AutoResetEvent(false);
            this.isInitialized = false;
        }

        /// <summary>
        /// Gets the parent unity container.
        /// </summary>
        public static UnityContainer ParentUnityContainer
        {
            get
            {
                return container;
            }
        }

        /// <summary>
        /// Starts the notification service.
        /// </summary>
        public void Start()
        {
            this.Run();
        }

        /// <summary>
        /// Stops the notification service.
        /// </summary>
        public void Stop()
        {
            Logger.Logger.Info("Stopping the notification service.");
            this.stop.Set();
        }

        /// <summary>
        /// Pauses the notification service.
        /// </summary>
        public void Pause()
        {
            Logger.Logger.Info("Pausing the notification service.");
            this.pause.Set();
        }

        /// <summary>
        /// Resumes the notification service.
        /// </summary>
        public void Resume()
        {
            Logger.Logger.Info("Continuing the notification service.");
            this.pause.Set();
        }
        
        /// <summary>
        /// Initialize the engine.
        /// </summary>
        public void Initialize()
        {
            try
            {
                Logger.Logger.Info("notification service loop is initializing.");
                ConcurrentQueue<TaskResult> resultsQueue = new ConcurrentQueue<TaskResult>();

                RegisterUnityContainer();

                CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSettingPublisher) =>
                {
                    configSettingPublisher(ConfigReader<string>.GetSetting(configName));
                });

                this.notificationTasksProc = new TaskProcessor(this.NextTaskFromNotificationQueue, resultsQueue);
                this.notificationTasksProc.Pause = 3600;
                this.notificationTasksProc.Start();

                this.isInitialized = true;
            }
            catch (Exception ex)
            {
                Logger.Logger.Critical(ex, "Service initialization failed");
                throw;
            }
        }

        /// <summary>
        /// Runs the notification service in a continuous loop.
        /// </summary>
        public void Run()
        {
            // If the Engine is not initialized, initialize now.
            if (!this.isInitialized)
            {
                this.Initialize();
            }

            Logger.Logger.Info("notification service is starting.");
            int minMillisecondsPerIteration = Constants.MinMillisecondsPerIteration;
            int maxMillisecondsPerIteration = Constants.MaxMillisecondsPerIteration;
            long minTicksPerIteration = (new TimeSpan(0, 0, 0, 0, minMillisecondsPerIteration)).Ticks;
            long maxTicksPerIteration = (new TimeSpan(0, 0, 0, 0, maxMillisecondsPerIteration)).Ticks;
            long nextIterationAt = DateTime.Now.Ticks;

            while (true)
            {
                // Check whether the service has received stop request before 
                // running thread goes to sleep
                if (true == this.stop.WaitOne(0))
                {
                    break;
                }

                TimeSpan tillNextIteration = new TimeSpan(nextIterationAt - DateTime.Now.Ticks);
                if (tillNextIteration.TotalMilliseconds > 0)
                {
                    System.Threading.Thread.Sleep(tillNextIteration);

                    // Check whether the service has received stop request after 
                    // running thread wake from sleep
                    if (true == this.stop.WaitOne(0))
                    {
                        break;
                    }
                }
#if DEBUG
                Console.WriteLine("Tick {0}", DateTime.Now);
#endif
                nextIterationAt = DateTime.Now.Ticks + maxTicksPerIteration;

                // If the service pause is initiated, wait until service resumes.
                if (true == this.pause.WaitOne(0))
                {
                    Logger.Logger.Info("notification service paused.");
                    this.pause.WaitOne(Timeout.Infinite);
                    Logger.Logger.Info("notification service resumes.");
                }
            }

            Logger.Logger.Info("notification service is stopping.");
            DateTime stoppedAt = DateTime.Now;

            // Stop the task processors and wait for them to complete their current job.
            this.notificationTasksProc.Stop();
            while (!this.notificationTasksProc.IsStopped && !this.notificationTasksProc.IsStopped)
            {
                double totalWaitingTime = stoppedAt.Subtract(DateTime.Now).TotalMilliseconds;
                if (totalWaitingTime >= Constants.ShutdownWaitTime)
                {
                    Logger.Logger.Info("Service is terminating after '{0}' ms waiting", Constants.ShutdownWaitTime);
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Returns the next available task pending on the "Notification" Azure queue.
        /// </summary>
        /// <returns>A task instance which may be null if the queue is currently empty.</returns>
        /// <remarks>
        /// This method attempts to read the message at the top of the Azure queue. If a
        /// message is present, it is converted to an appropriate ITask instance. If the
        /// conversion fails or the queue is empty, a null value is returned. 
        /// </remarks>
        public ITask NextTaskFromNotificationQueue()
        {
            ITask task = null;
            try
            {
                // CloudQueue queue = Storage.UpdateRequestsQueue;
                IQueueRepository queueRepository = DependencyResolver.Current.GetService(typeof(IQueueRepository)) as IQueueRepository;

                CloudQueue queue = queueRepository.NotificationQueue;
                CloudQueueMessage msg = queue.GetMessage();
                if (msg != null)
                {
                    Logger.Logger.Info("Got message from notifications queue.");
                    queue.DeleteMessage(msg);
                    object request = queueRepository.Unpack(msg);

                    if (Constants.SendMultipleMails)
                    {
                        Logger.Logger.Verbose("Creating MultipleEmailTask.");
                        task = new MultipleEmailTask(request);

                        return task;
                    }
                    else
                    {
                        Logger.Logger.Verbose("Creating EmailTask.");
                        task = new EmailTask(request);

                        return task;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Error(ex, "Failed to read a message from notifications queue.");
            }

            return task;
        }

        /// <summary>
        /// Creates an instance of UnityContainer and registers the instances which needs to be injected
        /// to Controllers/Views/Services, etc.
        /// </summary>
        private static void RegisterUnityContainer()
        {
            container = new UnityContainer();

            //// TODO: Need to check which lifetime manager to be used.

            container.RegisterType<EarthOnlineEntities>(
                    new PerRequestLifetimeManager(), new InjectionConstructor(ConfigReader<string>.GetSetting("EarthOnlineEntities")));

            container.RegisterType<IQueueRepository, QueueRepository>(new ContainerControlledLifetimeManager());

            container.RegisterType<ICommunityRepository, CommunityRepository>();
            container.RegisterType<IContentRepository, ContentRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<ICommunityCommentRepository, CommunityCommentRepository>();
            container.RegisterType<IContentCommentsRepository, ContentCommentsRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
