#nullable disable

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers
{
    internal static class BitmapExtensions
    {
        public static Stream SaveToStream(this Bitmap bitmap, ImageFormat format)
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, format);
            ms.Position = 0;
            return ms;
        }

        public static async Task SaveAsync(this Bitmap bitmap, IResponse response, ImageFormat format, CancellationToken token)
        {
            using var result = bitmap.SaveToStream(format);

            // We must copy to an intermediary stream at the moment so Content-Length gets set correctly on ASP.NET Core
            await result.CopyToAsync(response.OutputStream, token);
        }
    }
}
