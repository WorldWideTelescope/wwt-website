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
        {
            if (fileName is null)
            {
                return null;
            }

            var dataDir = ConfigurationManager.AppSettings["DataDir"];
            var jpeg = Path.Combine(dataDir, "thumbnails", fileName + ".jpg");

            if (!File.Exists(jpeg))
            {
                return File.OpenRead(jpeg);
            }

            return null;
        }
    }
}
