using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WWTWebservices;

namespace WWT.Azure
{
    /// <summary>
    /// An implementation of <see cref="IPlateTilePyramid"/> that adds awareness of the mars storage account
    /// </summary>
    public class MarsAwareSeekableAzurePlateTilePyramid : SeekableAzurePlateTilePyramid
    {
        private readonly Func<string, string, BlobClient> _blobRetriever;

        public MarsAwareSeekableAzurePlateTilePyramid(
            AzurePlateTilePyramidOptions options,
            AzureServiceAccessor services,
            ILogger<SeekableAzurePlateTilePyramid> logger)
            : base(options, services.WwtFiles, logger)
        {
            _blobRetriever = BuildLookup(services.Mars);
        }

        /// <summary>
        /// Overrides the default behavior to use the mars storage account when needed.
        /// </summary>
        protected override BlobClient GetBlob(string pathPrefix, string plateName)
            => _blobRetriever(pathPrefix, plateName) ?? base.GetBlob(pathPrefix, plateName);

        private static Func<string, string, BlobClient> BuildLookup(BlobServiceClient mars)
        {
            var cache = new ConcurrentDictionary<string, BlobClient>();

            var hirise = mars.GetBlobContainerClient("hirise");
            var moc = mars.GetBlobContainerClient("moc");

            // This maps known prefixes supplied to IPlateTilePyramid that are actually using the mars dataset.
            var marsCollection = new Dictionary<string, BlobContainerClient>
            {
                { @"\\wwt-mars\marsroot\dem\", mars.GetBlobContainerClient("dem") },
                { @"\\wwtfiles.file.core.windows.net\wwtmars\MarsDem", mars.GetBlobContainerClient("marsdem") },
                { @"\\wwt-mars\marsroot\hirise", hirise },
                { @"\\wwt-mars\marsroot\moc", moc },
                { @"https://marsstage.blob.core.windows.net/hirise", hirise },
                { @"https://marsstage.blob.core.windows.net/moc", moc },
                { @"\\wwt-mars\marsroot\MARSBASEMAP", mars.GetBlobContainerClient("marsbasemap") },
            };

            return (prefix, plateName) =>
                marsCollection.TryGetValue(prefix, out var marsContainer)
                ? cache.GetOrAdd(plateName, marsContainer.GetBlobClient)
                : null;
        }
    }
}