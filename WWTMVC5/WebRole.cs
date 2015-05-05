//-----------------------------------------------------------------------
// <copyright file="WebRole.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WWTMVC5
{
    public class WebRole : RoleEntryPoint
    {
        /*
        /// <summary>
        /// Called by Windows Azure to initialize the role instance.
        /// </summary>
        /// <returns>
        /// True if initialization succeeds, False if it fails. The default implementation returns True.
        /// </returns>
        /// 
        /*Diagnostics configuration in code is no longer supported - With the Azure SDK version 2.5, all diagnostics configuration must be done in the XML configuration file diagnostics.wadcfgx. Any previous code-based diagnostics configuration (for example, using the DiagnosticMonitor API) must be migrated to the diagnostics.wadcfgx file. Code used to configure crash dumps in previous SDKs must also be migrated to the diagnostics.wadcfgx file.
        public override bool OnStart()
        {
            
            //DiagnosticMonitorConfiguration dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();

            //dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            //dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;

            //dmc.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(60);

            //DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", dmc);
            return base.OnStart();
        }*/
    }
}