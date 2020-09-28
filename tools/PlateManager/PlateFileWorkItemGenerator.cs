using System;
using System.Collections.Generic;
using System.IO;
using WWTWebservices;
using WWT.Azure;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace PlateManager
{
    internal class PlateFileWorkItemGenerator : IWorkItemGenerator
    {
        private readonly AzurePlateTilePyramid _pyramid;
        private readonly ILogger<PlateFileWorkItemGenerator> _logger;

        public PlateFileWorkItemGenerator(AzurePlateTilePyramid pyramid, ILogger<PlateFileWorkItemGenerator> logger)
        {
            _pyramid = pyramid;
            _logger = logger;
        }

        public IEnumerable<Func<int, int, Task>> GenerateWorkItems(string plateFile, string baseUrl, CancellationToken token)
        {
            var filepart = Path.GetFileNameWithoutExtension(plateFile);
            var azureContainer = Path.GetFileName(plateFile).ToLowerInvariant();

            bool hasLevels = PlateTilePyramid.GetLevelCount(plateFile, out int levels);
            string thumbnail = plateFile.Replace(".plate", ".jpg").ToLower().Replace("-", "_");
            string wtmlfile = plateFile.Replace(".plate", ".wtml").ToLower();

            if (File.Exists(thumbnail))
            {
                _logger.LogTrace("Adding task for thumbnail {Path}", thumbnail);

                Task UploadThumbnail(int count, int total)
                {
                    return _pyramid.SaveStreamAsync(GetFileStream(thumbnail), azureContainer, filepart.ToLower().Replace("-", "_") + "_thumb.jpg", token);
                }

                yield return UploadThumbnail;
            }

            if (File.Exists(wtmlfile))
            {
                string wtmlFileOut = wtmlfile.Replace(".wtml", ".azure.wtml");
                string wtmldata = File.ReadAllText(wtmlfile);
                wtmldata = wtmldata.Replace(filepart + "/{1}/{3}/{3}_{2}.png", baseUrl + azureContainer + "/" + filepart + "L{1}X{2}Y{3}.png");
                wtmldata = wtmldata.Replace(filepart.ToLower().Replace("-", "_") + ".jpg", baseUrl + azureContainer + "/" + filepart.ToLower().Replace("-", "_") + "_thumb.jpg");
                File.WriteAllText(wtmlFileOut, wtmldata);
            }

            if (hasLevels)
            {
                _logger.LogInformation("Found {LevelCount} levels in {File}", levels, plateFile);

                int maxX = 1;
                int maxY = 1;
                for (int level = 0; level < levels; level++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        for (int x = 0; x < maxX; x++)
                        {
                            token.ThrowIfCancellationRequested();

                            var tmpLevel = level;
                            var tmpX = x;
                            var tmpY = y;

                            async Task UploadItem(int count, int total)
                            {
                                _logger.LogTrace("[{Count} of {Total}] Starting upload for {File} L{Level}X{X}Y{Y}", count, total, plateFile, tmpLevel, tmpX, tmpY);
                                try
                                {
                                    await ProcessPlateTileAsync(azureContainer, plateFile, tmpLevel, tmpX, tmpY, token);
                                    _logger.LogInformation("[{Count} of {Total}] Completed upload for {File} L{Level}X{X}Y{Y}", count, total, plateFile, tmpLevel, tmpX, tmpY);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, "[{Count} of {Total}] Unexpected error uploading {File} L{Level}X{X}Y{Y}", count, total, plateFile, tmpLevel, tmpX, tmpY);
                                }
                            }

                            yield return UploadItem;
                        }
                    }
                    maxX *= 2;
                    maxY *= 2;
                }

                _logger.LogInformation("Done adding upload tasks for {File}", plateFile);
            }
            else
            {
                _logger.LogWarning("Found no levels for {File}", plateFile);
            }
        }

        private Stream GetFileStream(string filename)
        {
            byte[] data = File.ReadAllBytes(filename);
            return new MemoryStream(data);
        }

        private async Task ProcessPlateTileAsync(string container, string plateFile, int level, int x, int y, CancellationToken token)
        {
            using var stream = PlateTilePyramid.GetFileStream(plateFile, level, x, y);

            if (stream != null)
            {
                await _pyramid.SaveStreamAsync(stream, container, level, x, y, token);
            }
        }
    }
}
