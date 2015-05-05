//-----------------------------------------------------------------------
// <copyright file="ProjectInstaller.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace Microsoft.Research.EarthOnline.NotificationService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected override void OnCommitted(IDictionary savedState)
        {
            try
            {
                base.OnCommitted(savedState);
                ServiceController serviceController = new System.ServiceProcess.ServiceController("Layerscape Notification Service");
                serviceController.Start();
            }
            catch (Exception)
            {
            }
        }
    }
}
