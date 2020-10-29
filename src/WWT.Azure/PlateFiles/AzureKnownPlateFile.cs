using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using WWT.PlateFiles;

namespace WWT.Azure
{
    public class AzureKnownPlateFile : IKnownPlateFiles
    {
        private readonly Dictionary<string, string> _map;

        public AzureKnownPlateFile(AzurePlateTilePyramidOptions options, BlobServiceClient service, ILogger<AzureKnownPlateFile> logger)
        {
            _map = BuildMap(options, service, logger);
        }

        public bool TryNormalizePlateName(string input, out string platefile)
            => _map.TryGetValue(input, out platefile);

        private static Dictionary<string, string> BuildMap(AzurePlateTilePyramidOptions options, BlobServiceClient service, ILogger<AzureKnownPlateFile> logger)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var blob = service.GetBlobContainerClient(options.Container).GetBlobClient(options.KnownPlateFile);

                using var stream = blob.OpenRead();
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();

                    if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                    {
                        result.Add(line, line);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected error while loading known plate files");
            }

            logger.LogInformation("Loaded {Count} known plate files", result.Count);

            return result;
        }
    }
}