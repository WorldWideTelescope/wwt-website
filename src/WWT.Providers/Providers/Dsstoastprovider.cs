#nullable disable

using System;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/DSSToast.aspx")]
    public class DSSToastProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTile;
        private readonly WwtOptions _options;

        public DSSToastProvider(IPlateTilePyramid plateTile, WwtOptions options)
        {
            _plateTile = plateTile;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level > 12)
            {
                await context.Response.WriteAsync("No image", token);
                context.Response.Close();
            }
            else if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (var s = await _plateTile.GetStreamAsync(_options.WwtTilesDir, "DSSToast.plate", level, tileX, tileY, token))
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

                var filename = $"DSSPngL5to12_x{X32}_y{Y32}.plate";

                using (var s = await _plateTile.GetStreamAsync(_options.DssToastPng, filename, L5, X5, Y5, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
        }
    }
}
