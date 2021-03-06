using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace PlateManager
{
    internal class BaseOptions
    {
        public string Storage { get; set; }

        public string AzureContainer { get; set; } = "coredata";

        public bool Interactive { get; set; }

        public LogLevel LogLevel { get; set; }

        public bool SkipExisting { get; set; }

        public FileInfo ErrorLog { get; set; }
    }
}