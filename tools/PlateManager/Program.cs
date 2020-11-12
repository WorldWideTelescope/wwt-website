using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlateManager.List;
using Serilog;
using Serilog.Events;
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
                CreateUploadCommand(),
                CreateDownloadCommand(),
                CreateListCommand(),
            };

            using (var current = System.Diagnostics.Process.GetCurrentProcess())
            {
                root.Name = current.ProcessName;
            }

            return root.InvokeAsync(args);
        }

        static Command CreateListCommand()
        {
            var command = new Command("list");

            AddDefaultOptions(command);

            command.Handler = CommandHandler.Create<ListCommandOptions>(Run);

            return command;
        }

        static Command CreateUploadCommand()
        {
            var command = new Command("upload")
            {
                new Option<IEnumerable<FileInfo>>(new[] { "--file", "-f" }) { IsRequired = true }.ExistingOnly(),
                new Option<bool>("--useplate2format"),
                new Option<string>("--container", () => AzurePlateTilePyramidOptions.DefaultContainer),
                new Option<bool>("--skip-existing", () => true),
                new Option<string>("--baseUrl", () => "baseUrl"),
                new Option<int>("--uploader-count", () => UploadProcessorOptions.DefaultUploaderCount),
            };

            AddDefaultOptions(command);

            command.Handler = CommandHandler.Create<UploadProcessorOptions>(Run);

            return command;
        }

        static Command CreateDownloadCommand()
        {
            var command = new Command("download")
            {
                new Option<string>("--plate") { IsRequired = true },
                new Option<DirectoryInfo>(new[] { "--output", "-o" }) { IsRequired = true },
                new Option<int>("--levels", () => 2),
            };

            AddDefaultOptions(command);

            command.Handler = CommandHandler.Create<Download.DownloadCommandOptions>(Run);

            return command;
        }

        private static void AddDefaultOptions(Command command)
        {
            command.Add(new Option<string>("--storage", () => "https://127.0.0.1:10000/devstoreaccount1/"));
            command.Add(new Option<bool>("--interactive"));
            command.Add(new Option<LogLevel>("--log-level", () => LogLevel.Information));
            command.Add(new Option<FileInfo>("--error-log"));
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

            services
                .AddAzureServices(opt =>
                {
                    opt.StorageAccount = options.Storage;
                })
                .AddPlateFiles(opt =>
                {
                    opt.CreateContainer = true;
                    opt.SkipIfExists = options.SkipExisting;
                    opt.OverwriteExisting = !options.SkipExisting;
                    opt.Container = options.AzureContainer;
                });

            await using var container = services.BuildServiceProvider();

            await container.GetRequiredService<ICommand>().RunAsync(default);
        }
    }
}
