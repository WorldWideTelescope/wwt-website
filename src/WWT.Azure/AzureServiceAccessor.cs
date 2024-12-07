using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WWT.Azure
{
    public class AzureServiceAccessor
    {
        private readonly Lazy<BlobServiceClient> _wwtFiles;
        private readonly Lazy<BlobServiceClient> _mars;

        public AzureServiceAccessor(IServiceProvider sp)
        {
            _wwtFiles = new Lazy<BlobServiceClient>(() => sp.GetRequiredKeyedService<BlobServiceClient>("WwtFiles"), true);
            _mars = new Lazy<BlobServiceClient>(() => sp.GetRequiredKeyedService<BlobServiceClient>("Mars"), true);
        }

        public BlobServiceClient WwtFiles => _wwtFiles.Value;

        public BlobServiceClient Mars => _mars.Value;
    }
}
