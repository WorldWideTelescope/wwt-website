using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WWT.Azure;
using WWT.PlateFiles;

namespace PlateManager.Download
{
    class DownloadCommand : ICommand
    {
        private readonly DownloadCommandOptions _options;
        private readonly IPlateTileDownloader _downloader;
        private readonly ILogger<DownloadCommand> _logger;

        public DownloadCommand(DownloadCommandOptions options, IPlateTileDownloader downloader, ILogger<DownloadCommand> logger)
        {
            _options = options;
            _downloader = downloader;
            _logger = logger;
        }

        private IAsyncEnumerable<string> GetAvailableContainers(CancellationToken token)
        {
            if (_options.Plate is null)
            {
                _logger.LogInformation("No plates were specified. Defaulting to all available.");

                return _downloader.GetPlateNames(token);
            }

            _logger.LogInformation("Using supplied plate names");

            return _options.Plate.ToAsyncEnumerable();
        }

        public async Task RunAsync(CancellationToken token)
        {
            if (!_options.Output.Exists)
            {
                _options.Output.Create();
            }

            await foreach (var plate in GetAvailableContainers(token).WithCancellation(token))
            {
                var outputFile = new FileInfo(Path.Combine(_options.Output.FullName, plate));

                if (_options.SkipExisting && outputFile.Exists)
                {
                    _logger.LogWarning("Plate {PlateFile} already exists. Skipping.");

                    continue;
                }

                _logger.LogInformation("Downloading {PlateFile}", outputFile.Name);

                try
                {
                    using (var fs = outputFile.OpenWrite())
                    {
                        fs.SetLength(0);

                        var count = await _downloader.DownloadPlateFileAsync(plate, fs, _options.Levels, token);

                        if (count == 0)
                        {
                            _logger.LogWarning("No content added for {PlateFile}", outputFile.Name);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unexpected error getting {PlateName}", outputFile.Name);

                    if (outputFile.Exists)
                    {
                        outputFile.Delete();
                    }
                }

                var size = FormatSize(outputFile.Length);

                _logger.LogInformation("Completed {PlateFile} download. Resulting file is {Size}", outputFile.Name, size);
            }
        }

        private readonly string[] sizes = new[] { "B", "KB", "MB", "GB", "TB" };

        private string FormatSize(double len)
        {
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}
