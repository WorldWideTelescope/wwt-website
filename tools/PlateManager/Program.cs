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
using System.Threading.Tasks;
using WWT.Azure;

namespace PlateManager
{
    class Program
    {
        static Task<int> Main(string[] args)
        {
            var root = new RootCommand
            {
                CreateUploadCommand()
            };

            using (var current = System.Diagnostics.Process.GetCurrentProcess())
            {
                root.Name = current.ProcessName;
            }

            return root.InvokeAsync(args);
        }

        static Command CreateUploadCommand()
        {
            var command = new Command("upload")
            {
                new Option<IEnumerable<FileInfo>>(new[] { "--file", "-f" }) { IsRequired = true }.ExistingOnly(),
                new Option<Uri>("--storage", () => new Uri("https://127.0.0.1:10000/devstoreaccount1/")),
                new Option<bool>("--useplate2format"),
                new Option<string>("--container", () => "plate-data"),
                new Option<bool>("--interactive"),
                new Option<bool>("--skip-existing", () => true),
                new Option<LogLevel>("--log-level", () => LogLevel.Information),
                new Option<string>("--baseUrl", () => "baseUrl"),
                new Option<int>("--uploader-count", () => UploadProcessorOptions.DefaultUploaderCount),
                new Option<FileInfo>("--error-log")
            };

            command.Handler = CommandHandler.Create<UploadProcessorOptions>(Run);

            return command;
        }

        static async Task Run<T>(T options)
            where T : BaseOptions, IServiceRegistration
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.AddFilter("Azure-Core", LogLevel.Error);
                builder.SetMinimumLevel(options.LogLevel);
                builder.AddConsole();

                if (options.ErrorLog != null)
                {
                    var serilog = new LoggerConfiguration()
                        .WriteTo.File(options.ErrorLog.FullName, LogEventLevel.Error)
                        .CreateLogger();

                    builder.AddSerilog(serilog);
                }
            });

            options.AddServices(services);

            services.AddSingleton(options);
            services.AddSingleton<AzurePlateTilePyramid>();

            services.AddSingleton(new AzurePlateTilePyramidOptions
            {
                CreateContainer = true,
                SkipIfExists = options.SkipExisting,
                OverwriteExisting = !options.SkipExisting
            });

            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(options.Storage);

                if (options.Interactive)
                {
                    builder.UseCredential(new InteractiveBrowserCredential());
                }
                else
                {
                    builder.UseCredential(new DefaultAzureCredential());
                }
            });

            using var container = services.BuildServiceProvider();

            await container.GetRequiredService<ICommand>().RunAsync(default);
        }
    }
}
