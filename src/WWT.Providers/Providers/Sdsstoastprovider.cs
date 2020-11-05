using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class SDSSToastProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;
        private readonly IOctTileMapBuilder _octTileMap;

        public SDSSToastProvider(IPlateTilePyramid plateTiles, WwtOptions options, IOctTileMapBuilder octTileMap)
        {
            _plateTiles = plateTiles;
            _options = options;
            _octTileMap = octTileMap;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            if (context.Request.UserAgent.ToLower().Contains("wget"))
            {
                context.Response.Write("You are not allowed to bulk download imagery thru the tile service. Please contact wwtpage@microsoft.com for more information.");
                context.Response.End();
                return;
            }

            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            //++
            // 2014-09-26 security fix.
            //
            int level = 0;
            int tileX = 0;
            int tileY = 0;
            try
            {
                level = Convert.ToInt32(values[0]);
                tileX = Convert.ToInt32(values[1]);
                tileY = Convert.ToInt32(values[2]);
            }
            catch
            {
                context.Response.Write("Invalid query string.");
                context.Response.End();
                return;
            }

            string wwtTilesDir = _options.WwtTilesDir;

            if (level > 14)
            {
                context.Response.Write("No image");
                context.Response.End();
                return;
            }

            if (level < 9)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(wwtTilesDir, "SDSS_8.plate", level, tileX, tileY, token))
                {
                    if (s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                        return;
                    }

                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }

            using (var stream = await _octTileMap.GetOctTileAsync(level, tileX, tileY, enforceBoundary: true, token: token))
            {
                if (stream is null)
                {
                    context.Response.Write("No Image");
                }
                else
                {
                    await stream.CopyToAsync(context.Response.OutputStream, token);
                }
            }
        }
    }
}
