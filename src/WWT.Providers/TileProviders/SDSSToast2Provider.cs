#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/SDSSToast2.aspx")]
    public class SDSSToast2Provider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;
        private readonly IOctTileMapBuilder _octTileMap;

        public SDSSToast2Provider(IPlateTilePyramid plateTiles, WwtOptions options, IOctTileMapBuilder octTileMap)
        {
            _plateTiles = plateTiles;
            _options = options;
            _octTileMap = octTileMap;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            if (level > 14)
            {
                await context.Response.WriteAsync("No image", token);
                context.Response.Close();
                return;
            }

            if (level < 9)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "SDSS_8.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }

            using (var stream = await _octTileMap.GetOctTileAsync(level, tileX, tileY, enforceBoundary: true, token: token))
            {
                if (stream is null)
                {
                    await context.Response.WriteAsync("No image", token);
                }
                else
                {
                    await stream.CopyToAsync(context.Response.OutputStream, token);
                }
            }
        }
    }
}
