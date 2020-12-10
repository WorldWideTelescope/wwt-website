#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/MartianTile.aspx")]
    [RequestEndpoint("/wwtweb/MartianTileNew.aspx")]
    public class MartianTileProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;

        public MartianTileProvider(IPlateTilePyramid plateTiles)
        {
            _plateTiles = plateTiles;
        }

        public override string ContentType => ContentTypes.Text;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY, var dataset) = await HandleLXYExtraQParameter(context, token);
            if (errored)
                return;

            context.Response.ContentType = "image/png";

            switch (dataset)
            {
                case "mars_base_map":
                    if (level < 18)
                    {
                        using (Stream s = await _plateTiles.GetStreamAsync(@"F:\WWTTiles", "marsbasemap.plate", -1, level, tileX, tileY, token))
                        {
                            if (s == null || (int)s.Length == 0)
                            {
                                context.Response.Clear();
                                context.Response.ContentType = "text/plain";
                                await context.Response.WriteAsync("No image", token);
                                context.Response.End();
                            }
                            else
                            {
                                await s.CopyToAsync(context.Response.OutputStream, token);
                                context.Response.Flush();
                                context.Response.End();
                            }
                        }
                    }
                    break;
                case "mars_terrain_color":
                    {
                        using (var s = await _plateTiles.GetStreamAsync("https://wwtfiles.blob.core.windows.net/marsmola", "marsmola.plate", level, tileX, tileY, token))
                        {
                            if (s == null || (int)s.Length == 0)
                            {
                                context.Response.Clear();
                                context.Response.ContentType = "text/plain";
                                await context.Response.WriteAsync("No image", token);
                                context.Response.End();
                            }
                            else
                            {
                                await s.CopyToAsync(context.Response.OutputStream, token);
                                context.Response.Flush();
                                context.Response.End();
                            }
                        }
                    }
                    break;
                default:
                    {
                        context.Response.StatusCode = 404;
                    }
                    break;
            }
        }
    }
}
