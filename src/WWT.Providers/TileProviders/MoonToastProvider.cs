#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/moontoast.aspx")]
    public class MoontoastProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public MoontoastProvider(IPlateTilePyramid plateTiles, WwtOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string wwtTilesDir = Path.Combine(_options.WwtTilesDir, "LROWAC");

            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            if (level > 10)
            {
                await context.Response.WriteAsync("No image", token);
                context.Response.Close();
                return; ;
            }

            if (level < 7)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(wwtTilesDir, "LROWAC_L0X0Y0.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return; ;
                }
            }
            else
            {
                int powLev5Diff = (int)Math.Pow(2, level - 3);
                int X32 = tileX / powLev5Diff;
                int Y32 = tileY / powLev5Diff;

                int L5 = level - 3;
                int X5 = tileX % powLev5Diff;
                int Y5 = tileY % powLev5Diff;
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(wwtTilesDir, $"LROWAC_L3X{X32}Y{Y32}.plate", L5, X5, Y5, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return; ;
                }
            }
        }
    }
}
