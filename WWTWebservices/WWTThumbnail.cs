using System.Configuration;
using System.IO;

namespace WWTThumbnails
{
    public class WWTThumbnail
    {
        private const string DefaultThumbnail = "Star";

        public static Stream GetThumbnailStream(string name, string type)
        {
            return GetThumbnailStream(name) ?? GetThumbnailStream(type) ?? GetThumbnailStream(DefaultThumbnail);
        }

        private static Stream GetThumbnailStream(string fileName)
            => GetThumbnailStreamFromFile(fileName, "fromAssembly") ?? GetThumbnailStreamFromFile(fileName, "fromBackup");

        private static Stream GetThumbnailStreamFromFile(string fileName, string sub)
        {
            if (fileName is null)
            {
                return null;
            }

            var jpeg = Path.Combine(ConfigurationManager.AppSettings["DataDir"], "thumbnails", sub, fileName + ".jpg");

            if (!File.Exists(jpeg))
            {
                return File.OpenRead(jpeg);
            }

            return null;
        }
    }
}
