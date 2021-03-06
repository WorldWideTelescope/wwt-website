#nullable disable

using System;
using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/GalexToast.aspx")]
    public class GalexToastProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public GalexToastProvider(IPlateTilePyramid plateTiles, WwtOptions options)
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

            if (level > 10)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("No image", token);
                context.Response.End();
                return;
            }

            if (level < 9)
            {
                try
                {
                    context.Response.ContentType = "image/png";

                    using (var s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "GalexBoth_L0to8_x0_y0.plate", level, tileX, tileY, token))
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
                    int powLev3Diff = (int)Math.Pow(2, level - 3);
                    int X8 = tileX / powLev3Diff;
                    int Y8 = tileY / powLev3Diff;

                    int L3 = level - 3;
                    int X3 = tileX % powLev3Diff;
                    int Y3 = tileY % powLev3Diff;

                    context.Response.ContentType = "image/png";

                    using (var s = await _plateTiles.GetStreamAsync(_options.WwtGalexDir, $"GalexBoth_L3to10_x{X8}_y{Y8}.plate", L3, X3, Y3, token))
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
        }
    }
}
