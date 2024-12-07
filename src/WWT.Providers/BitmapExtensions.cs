using SixLabors.ImageSharp;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    internal static class BitmapExtensions
    {
        public static Stream ToPngStream(this Image bitmap)
        {
            var ms = new MemoryStream();
            bitmap.SaveAsPng(ms);
            ms.Position = 0;
            return ms;
        }

        public static Stream ToJpegStream(this Image bitmap)
        {
            var ms = new MemoryStream();
            bitmap.SaveAsJpeg(ms);
            ms.Position = 0;
            return ms;
        }

        public static Activity StartImageProcessing(this ActivitySource source, [CallerMemberName] string name = null)
        {
            if (source.StartActivity(name) is { } activity)
            {
                activity.AddBaggage("ImageProcessing", "true");
                return activity;
            }

            return null;
        }

        public static async Task SavePngResponseAsync(this Image bitmap, IResponse response, CancellationToken token)
        {
            using var result = bitmap.ToPngStream();

            // We must copy to an intermediary stream at the moment so Content-Length gets set correctly on ASP.NET Core
            await result.CopyToAsync(response.OutputStream, token);
        }
    }
}
