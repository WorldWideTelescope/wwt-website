#nullable disable

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;
using WWTWebservices;

namespace WWT.Azure
{
    public class AzurePlateFileDownloader : IPlateTileDownloader
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly ILogger<AzurePlateFileDownloader> _logger;

        public AzurePlateFileDownloader(IPlateTilePyramid plateTiles, ILogger<AzurePlateFileDownloader> logger)
        {
            _plateTiles = plateTiles;
            _logger = logger;
        }

        public async Task<int> DownloadPlateFileAsync(string name, Stream stream, int maxLevel, CancellationToken token)
        {
            using var file = new PlateTilePyramid(stream, maxLevel);

            for (int level = 0; level < maxLevel; level++)
            {
                var size = Math.Pow(2, level);

                _logger.LogInformation("Adding level {Level} to {Plate} ({ImageCount} images)", level, name, size * size);

                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        using var tile = await _plateTiles.GetStreamAsync(string.Empty, name, level, x, y, token);

                        await file.AddStreamAsync(tile, level, x, y, token);
                    }
                }
            }

            file.UpdateHeaderAndClose();

            return file.Count;
        }

        public IAsyncEnumerable<string> GetPlateNames(CancellationToken token) => _plateTiles.GetPlateNames(token);
    }
}
