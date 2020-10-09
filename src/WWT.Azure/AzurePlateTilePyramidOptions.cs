namespace WWT.Azure
{
    public class AzurePlateTilePyramidOptions
    {
        public bool CreateContainer { get; set; }

        public string Container { get; set; } = "plate-data";

        public bool SkipIfExists { get; set; }

        public bool OverwriteExisting { get; set; }
    }
}