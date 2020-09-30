using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WWT.Azure;

namespace PlateManager
{
    class Program
    {
        static Task<int> Main(string[] args)
        {
            var command = new Command("upload")
            {
                new Option<IEnumerable<FileInfo>>(new[] { "--file", "-f" }) { IsRequired = true }.ExistingOnly(),
                new Option<Uri>("--storage", () => new Uri("https://127.0.0.1:10000/devstoreaccount1/")),
                new Option<bool>("--useplate2format"),
                new Option<bool>("--interactive"),
                new Option<bool>("--skip-existing", () => true),
                new Option<LogLevel>("--log-level", () => LogLevel.Information),
                new Option<string>("--baseUrl", () => "baseUrl"),
                new Option<FileInfo>("--error-log")
            };

            command.Handler = CommandHandler.Create<UploadOptions>(Run);

            var root = new RootCommand
            {
                command
            };

            using (var current = System.Diagnostics.Process.GetCurrentProcess())
            {
                root.Name = current.ProcessName;
            }

            return root.InvokeAsync(args);
        }

        private class UploadOptions
        {
            public IEnumerable<FileInfo> File { get; set; }

            public Uri Storage { get; set; }

            public bool Interactive { get; set; }

            public bool UsePlate2Format { get; set; }

            public string BaseUrl { get; set; }

            public bool SkipExisting { get; set; }

            public LogLevel LogLevel { get; set; }

            public FileInfo ErrorLog { get; set; }
        }

        static async Task Run(UploadOptions uploadOptions)
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.AddFilter("Azure-Core", LogLevel.Error);
                builder.SetMinimumLevel(uploadOptions.LogLevel);
                builder.AddConsole();

                if (uploadOptions.ErrorLog != null)
                {
                    var serilog = new LoggerConfiguration()
                        .WriteTo.File(uploadOptions.ErrorLog.FullName, LogEventLevel.Error)
                        .CreateLogger();

                    builder.AddSerilog(serilog);
                }
            });

            if (uploadOptions.UsePlate2Format)
            {
                services.AddTransient<IWorkItemGenerator, PlateFile2WorkItemGenerator>();
            }
            else
            {
                services.AddTransient<IWorkItemGenerator, PlateFileWorkItemGenerator>();
            }

            services.AddSingleton<AzurePlateTilePyramid>();
            services.AddSingleton<Processor>();

            services.AddSingleton(new AzurePlateTilePyramidOptions
            {
                CreateContainer = true,
                SkipIfExists = uploadOptions.SkipExisting,
            });

            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(uploadOptions.Storage);

                if (uploadOptions.Interactive)
                {
                    builder.UseCredential(new InteractiveBrowserCredential());
                }
                else
                {
                    builder.UseCredential(new DefaultAzureCredential());
                }
            });

            var options = new ProcessorOptions
            {
                BaseUrl = uploadOptions.BaseUrl,
                Files = uploadOptions.File.Select(p => p.FullName)
            };

            using var container = services.BuildServiceProvider();

            await container.GetRequiredService<Processor>().ProcessAsync(options, default);
        }
    }
}
