using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers
{
    internal static class ImageSharpExtensions
    {
        public static async Task RespondPngAsync(this Image img, IResponse response, CancellationToken token)
        {
            var ms = new MemoryStream();
            img.Save(ms, new PngEncoder());
            ms.Position = 0;

            response.ContentType = "image/png";
            response.AddHeader("Content-Length", ms.Length.ToString());
            await ms.CopyToAsync(response.OutputStream, token);
            response.Flush();
            response.End();
        }
    }
}
