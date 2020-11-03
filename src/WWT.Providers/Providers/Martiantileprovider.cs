using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class MartianTileProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;

        public MartianTileProvider(IPlateTilePyramid plateTiles)
        {
            _plateTiles = plateTiles;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];

            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            context.Response.ContentType = "image/png";

            switch (values[3])
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
                                context.Response.Write("No image");
                                context.Response.End();
                            }
                            else
                            {
                                await s.CopyToAsync(context.Response.OutputStream);
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
                                context.Response.Write("No image");
                                context.Response.End();
                            }
                            else
                            {
                                await s.CopyToAsync(context.Response.OutputStream);
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
