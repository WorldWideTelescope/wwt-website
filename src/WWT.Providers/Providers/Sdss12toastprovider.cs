using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/SDSS12Toast.aspx")]
    public class SDSS12ToastProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;
        private readonly IOctTileMapBuilder _octTileMap;

        public SDSS12ToastProvider(IPlateTilePyramid plateTiles, WwtOptions options, IOctTileMapBuilder octTileMap)
        {
            _plateTiles = plateTiles;
            _options = options;
            _octTileMap = octTileMap;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            if (context.Request.UserAgent.ToLower().Contains("wget"))
            {
                await context.Response.WriteAsync("You are not allowed to bulk download imagery thru the tile service. Please contact wwtpage@microsoft.com for more information.", token);
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
                await context.Response.WriteAsync("Invalid query string.", token);
                context.Response.End();
                return;
            }

            if (level > 14)
            {
                await context.Response.WriteAsync("No image", token);
                context.Response.End();
                return;
            }

            if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "sdssdr12_7.plate", level, tileX, tileY, token))
                {
                    if (s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("No image", token);
                        context.Response.End();
                        return;
                    }

                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }

            context.Response.ContentType = "image/png";

            using (var stream = await _octTileMap.GetOctTileAsync(level, tileX, tileY, token: token))
            {
                await stream.CopyToAsync(context.Response.OutputStream);
            }
        }
    }
}
