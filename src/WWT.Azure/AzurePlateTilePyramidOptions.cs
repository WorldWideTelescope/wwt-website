namespace WWT.Azure
{
    public class AzurePlateTilePyramidOptions
    {
        public const string DefaultContainer = "coredata";

        public bool CreateContainer { get; set; }

        public string Container { get; set; } = DefaultContainer;

        public bool SkipIfExists { get; set; }

        public bool OverwriteExisting { get; set; }

        /// <summary>
        /// TODO: The current storage account is a classic storage account and managed identity is not supported
        /// If the setting is a URL, we'll use managed identity. Otherwise, we'll expect it to be a connection string.
        /// </summary>
        public string StorageAccount { get; set; }

        public bool UseAzurePlateFiles { get; set; }
    }
}