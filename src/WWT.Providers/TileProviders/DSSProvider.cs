#nullable disable

using System;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/DSS.aspx")]
    public class DSSProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTile;
        private readonly WwtOptions _options;

        public DSSProvider(IPlateTilePyramid plateTile, WwtOptions options)
        {
            _plateTile = plateTile;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            if (level > 12)
            {
                await context.Response.WriteAsync("No image", token);
                context.Response.Close();
            }
            else if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (var s = await _plateTile.GetStreamAsync(_options.WwtTilesDir, "DSSTerraPixel.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            else
            {
                int powLev5Diff = (int)Math.Pow(2, level - 5);
                int X32 = tileX / powLev5Diff;
                int Y32 = tileY / powLev5Diff;

                int L5 = level - 5;
                int X5 = tileX % powLev5Diff;
                int Y5 = tileY % powLev5Diff;

                context.Response.ContentType = "image/png";

                string filename = $"DSSPngL5to12_x{X32}_y{Y32}.plate";

                using (var s = await _plateTile.GetStreamAsync(_options.DssTerapixelDir, filename, L5, X5, Y5, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
        }
    }
}
