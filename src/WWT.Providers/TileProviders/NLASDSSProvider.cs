// NOIRLab All Sky combined with WWT DSS
//
// This needs a custom tile provider because layers 0-8 use a custom fade of the
// NOIRLab All-Sky map into the classic WWT DSS imagery, then layers 9-12 use
// the classic WWT DSS imagery.

#nullable disable

using System;
using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/nlasdss.aspx")]
    public class NLASDSSProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTile;
        private readonly WwtOptions _options;

        public NLASDSSProvider(IPlateTilePyramid plateTile, WwtOptions options)
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
            else if (level < 9)
            {
                // Levels 0-8 go to the blended plate.

                context.Response.ContentType = "image/png";

                using (var s = await _plateTile.GetStreamAsync(_options.WwtTilesDir, "nlas_dss_rev1.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            else
            {
                // Levels 9-12 are handled exactly like classic DSS.

                int powLev5Diff = (int) Math.Pow(2, level - 5);
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
