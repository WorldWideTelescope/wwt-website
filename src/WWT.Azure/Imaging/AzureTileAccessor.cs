#nullable disable

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.Imaging;

namespace WWT.Azure
{
    public class AzureTileOptions
    {
        public string ContainerName { get; set; }
    }

    public class AzureTileAccessor : ITileAccessor
    {
        private readonly BlobContainerClient _container;
        private readonly ILogger<AzureTileAccessor> _logger;

        public AzureTileAccessor(AzureTileOptions options, BlobServiceClient service, ILogger<AzureTileAccessor> logger)
        {
            _container = service.GetBlobContainerClient(options.ContainerName);
            _logger = logger;
        }

        public ITileCreator CreateTile(string id)
            => new AzureTileCreator(id, _container);

        private class AzureTileCreator : ITileCreator
        {
            private readonly string _id;
            private readonly BlobContainerClient _container;

            public AzureTileCreator(string id, BlobContainerClient container)
            {
                _id = id;
                _container = container;
            }

            public async Task<bool> ExistsAsync(CancellationToken token)
            {
                var blob = _container.GetBlobClient(GetThumbnailName(_id));
                var exists = await blob.ExistsAsync(token).ConfigureAwait(false);

                return exists.Value;
            }

            public async Task AddThumbnailAsync(Stream thumbnail, CancellationToken token)
            {
                var blob = _container.GetBlobClient(GetThumbnailName(_id));

                await blob.UploadAsync(thumbnail, token);
            }

            public Task AddTileAsync(Stream tile, int level, int x, int y, CancellationToken token)
            {
                var blob = _container.GetBlobClient(GetTileName(_id, level, x, y));

                return blob.UploadAsync(tile, token);
            }
        }

        public Task<Stream> GetThumbnailAsync(string name, CancellationToken token)
            => GetBlob(GetThumbnailName(name), token);

        public Task<Stream> GetTileAsync(string id, int level, int x, int y, CancellationToken token)
            => GetBlob(GetTileName(id, level, x, y), token);

        internal static string GetTileName(string id, int level, int x, int y)
            => $"imagesTiler/{id}/{level}/{y}/{y}_{x}.png";

        internal static string GetThumbnailName(string name)
            => $"imagesTiler/thumbnails/{name}.jpg";

        private async Task<Stream> GetBlob(string name, CancellationToken token)
        {
            var blob = _container.GetBlobClient(name);

            try
            {
                return await blob.OpenReadAsync(new BlobOpenReadOptions(allowModifications: false), token);
            }
            catch (RequestFailedException e)
            {
                _logger.LogError(e, "Could not fine requested tile");
                return null;
            }
        }
    }
}
