using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.Azure;

namespace PlateManager
{
    internal sealed class PlateFile2WorkItemGenerator : PlateFileWorkItemGeneratorBase, IWorkItemGenerator
    {
        private readonly AzurePlateTilePyramid _pyramid;
        private readonly ILogger<PlateFile2WorkItemGenerator> _logger;

        public PlateFile2WorkItemGenerator(AzurePlateTilePyramid pyramid, ILogger<PlateFile2WorkItemGenerator> logger)
        {
            _pyramid = pyramid;
            _logger = logger;
        }

        public IEnumerable<Func<int, int, CancellationToken, Task>> GenerateWorkItems(string plateFile, string baseUrl, string container)
        {
            var filepart = Path.GetFileNameWithoutExtension(plateFile);
            var azureContainer = $"{container}//{Path.GetFileName(plateFile).ToLowerInvariant()}";

            // Handle thumbnails if one exists alongside the plate file
            string thumbnail = GetThumbnailName(plateFile);
            if (File.Exists(thumbnail))
            {
                _logger.LogTrace("Adding task for thumbnail {Path}", thumbnail);
                Task UploadThumbnail(int count, int total, CancellationToken token) => _pyramid.SaveStreamAsync(GetFileStream(thumbnail), azureContainer, GetThumbnailBlobName(filepart), token);
                yield return UploadThumbnail;
            }

            // Re-write entries in the WTML file to point to the new location for the images
            // to point to Azure if the WTML file exists alongside the plate file
            string wtmlfile = GetWtmlName(plateFile);
            if (File.Exists(wtmlfile))
            {
                string wtmlFileOut = wtmlfile.Replace(".wtml", ".azure.wtml");
                string wtmldata = UpdateWtmlEntries(File.ReadAllText(wtmlfile), filepart, baseUrl, azureContainer);
                File.WriteAllText(wtmlFileOut, wtmldata);
            }

            async Task UploadItem(int count, int total, CancellationToken token)
            {
                _logger.LogTrace("[{Count} of {Total}] Starting upload for {File}", count, total, plateFile);

                using (var fs = File.OpenRead(plateFile))
                {
                    await _pyramid.SaveStreamAsync(fs, plateFile, "", token).ConfigureAwait(false);
                }

                _logger.LogTrace("[{Count} of {Total}] Completed upload for {File}", count, total, plateFile);
            }

            yield return UploadItem;
            _logger.LogInformation("Done adding upload tasks for {File}", plateFile);
        }
    }
}
