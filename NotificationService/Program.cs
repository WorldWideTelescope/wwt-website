//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ServiceProcess;

namespace Microsoft.Research.EarthOnline.NotificationService
{
    public class Program
    {
        public static void Main()
        {
            ServiceBase[] servicesToRun = null;

            servicesToRun = new ServiceBase[] 
            {
                new NotificationService() 
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}
