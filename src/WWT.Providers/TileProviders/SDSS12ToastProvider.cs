#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

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

            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

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

                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }

            context.Response.ContentType = "image/png";

            using (var stream = await _octTileMap.GetOctTileAsync(level, tileX, tileY, token: token))
            {
                await stream.CopyToAsync(context.Response.OutputStream, token);
            }
        }
    }
}
