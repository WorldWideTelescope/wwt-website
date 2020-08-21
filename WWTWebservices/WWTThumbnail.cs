using System.Drawing;
using System.IO;
using System.Reflection;

namespace WWTThumbnails
{
	public class WWTThumbnail
	{
		public static Bitmap GetThumbnail(string fileName)
		{
			fileName = "WWTThumbnails.thumbnails." + fileName.ToLower() + ".jpg";
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
			{
				if (stream == null)
				{
					return null;
				}
				return new Bitmap(stream);
			}
		}

		public static Stream GetThumbnailStream(string fileName)
		{
			fileName = "WWTThumbnails.thumbnails." + fileName.ToLower() + ".jpg";
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
		}
	}
}
