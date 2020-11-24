#nullable disable

namespace WWT.Azure
{
    public class AzureOptions
    {
        /// <summary>
        /// TODO: The current storage account is a classic storage account and managed identity is not supported
        /// If the setting is a URL, we'll use managed identity. Otherwise, we'll expect it to be a connection string.
        /// </summary>
        public string StorageAccount { get; set; }

        /// <summary>
        /// TODO: The current storage account is a classic storage account and managed identity is not supported
        /// If the setting is a URL, we'll use managed identity. Otherwise, we'll expect it to be a connection string.
        /// </summary>
        public string MarsStorageAccount { get; set; }
    }
}
