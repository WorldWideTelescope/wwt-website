//-----------------------------------------------------------------------
// <copyright file="NotificationService.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ServiceProcess;
using System.Threading;
using Microsoft.Research.EarthOnline.NotificationService;
using Microsoft.Research.EarthOnline.NotificationService.Notification;

namespace Microsoft.Research.EarthOnline.NotificationService
{
    public partial class NotificationService : ServiceBase
    {
        private NotificationEngine engine;

        public NotificationService()
        {
            InitializeComponent();
            this.engine = new NotificationEngine();
        }

        protected override void OnStart(string[] args)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.StartService));
        }

        protected override void OnStop()
        {
            this.engine.Stop();
        }

        protected override void OnPause()
        {
            this.engine.Pause();
        }

        protected override void OnContinue()
        {
            this.engine.Resume();
        }

        private void StartService(object state)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            this.engine.Start();
        }
    }
}
