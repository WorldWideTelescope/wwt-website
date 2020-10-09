using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace PlateManager
{
    internal class BaseOptions
    {
        public Uri Storage { get; set; }

        public string AzureContainer { get; set; } = "plate-data";

        public bool Interactive { get; set; }

        public LogLevel LogLevel { get; set; }

        public bool SkipExisting { get; set; }

        public FileInfo ErrorLog { get; set; }
    }
}