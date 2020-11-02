using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class EarthMerBathProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;
        private readonly IVirtualEarthDownloader _veDownloader;

        public EarthMerBathProvider(IPlateTilePyramid plateTiles, FilePathOptions options, IVirtualEarthDownloader veDownloader)
        {
            _plateTiles = plateTiles;
            _options = options;
            _veDownloader = veDownloader;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level > 20)
            {
                context.Response.Write("No image");
                context.Response.Close();
            }
            else if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "BmngMerBase.plate", level, tileX, tileY, token))
                {
                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            else if (level < 10)
            {
                int L = level;
                int X = tileX;
                int Y = tileY;
                int powLev5Diff = (int)Math.Pow(2, L - 2);
                int X32 = X / powLev5Diff;
                int Y32 = Y / powLev5Diff;

                int L5 = L - 2;
                int X5 = X % powLev5Diff;
                int Y5 = Y % powLev5Diff;

                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, $"BmngMerL2X{X32}Y{Y32}.plate", L5, X5, Y5, token))
                {
                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            else
            {
                using var veTile = await _veDownloader.DownloadVeTileAsync(VirtualEarthTile.Ortho, level, tileX, tileY, token);

                await veTile.CopyToAsync(context.Response.OutputStream, token);
            }
        }
    }
}
