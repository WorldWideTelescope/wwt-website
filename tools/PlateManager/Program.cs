using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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
                new Option<bool>("--interactive"),
                new Option<string>("--baseUrl", ()=>"baseUrl")
            };

            command.Handler = CommandHandler.Create<IEnumerable<FileInfo>, Uri, bool, string>(Run);

            var root = new RootCommand
            {
                command
            };

            return root.InvokeAsync(args);
        }

        static async Task Run(IEnumerable<FileInfo> file, Uri storage, bool interactive, string baseUrl)
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            services.AddTransient<IWorkItemGenerator, PlateFileWorkItemGenerator>();

            services.AddSingleton<AzurePlateTilePyramid>();
            services.AddSingleton<Processor>();

            services.AddSingleton(new AzurePlateTilePyramidOptions
            {
                CreateContainer = true,
                OverwriteExisting = true
            });

            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(storage);

                if (interactive)
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
                BaseUrl = baseUrl,
                Files = file.Select(p => p.FullName)
            };

            using var container = services.BuildServiceProvider();

            await container.GetRequiredService<Processor>().ProcessAsync(options, default);
        }
    }
}
