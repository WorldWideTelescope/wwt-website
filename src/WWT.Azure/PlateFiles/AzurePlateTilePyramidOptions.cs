#nullable disable

namespace WWT.Azure
{
    public class AzurePlateTilePyramidOptions
    {
        public const string DefaultContainer = "coredata";

        public bool CreateContainer { get; set; }

        public string Container { get; set; } = DefaultContainer;

        public bool SkipIfExists { get; set; }

        public string KnownPlateFile { get; set; }

        public bool OverwriteExisting { get; set; }

        public bool UseAzurePlateFiles { get; set; }
    }
}
