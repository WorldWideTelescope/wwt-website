#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/Mipsgal.aspx")]
    public class MipsgalProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public MipsgalProvider(IPlateTilePyramid plateTiles, WwtOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            if (level > 11)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("No image", token);
                context.Response.End();
                return;
            }

            if (level < 11)
            {
                try
                {
                    context.Response.ContentType = "image/png";
                    using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "mipsgal_L0to10_x0_y0.plate", level, tileX, tileY, token))
                    {
                        await s.CopyToAsync(context.Response.OutputStream, token);
                        context.Response.Flush();
                        context.Response.End();
                        return;
                    }
                }
                catch
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("No image", token);
                    context.Response.End();
                    return;
                }
            }
            else
            {
                try
                {
                    int L = level;
                    int X = tileX;
                    int Y = tileY;
                    int powLev3Diff = (int)Math.Pow(2, L - 1);
                    int X8 = X / powLev3Diff;
                    int Y8 = Y / powLev3Diff;

                    int L3 = L - 1;
                    int X3 = X % powLev3Diff;
                    int Y3 = Y % powLev3Diff;
                    context.Response.ContentType = "image/png";

                    using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, $"mipsgal_L1to11_x{X8}_y{Y8}.plate", L3, X3, Y3, token))
                    {
                        await s.CopyToAsync(context.Response.OutputStream, token);
                        context.Response.Flush();
                        context.Response.End();
                        return;
                    }
                }
                catch
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("No image", token);
                    context.Response.End();
                }
            }
        }
    }
}
