using System.IO;

namespace WWTWebservices
{
    public class ConfigurationManagerFilePlateTilePyramid : IPlateTilePyramid
    {
        public Stream GetStream(string pathPrefix, string plateName, int level, int x, int y)
        {
            if (string.IsNullOrEmpty(pathPrefix))
            {
                throw new System.ArgumentException($"'{nameof(pathPrefix)}' cannot be null or empty", nameof(pathPrefix));
            }

            return PlateTilePyramid.GetFileStream(Path.Combine(pathPrefix, plateName), level, x, y);
        }
    }
}
