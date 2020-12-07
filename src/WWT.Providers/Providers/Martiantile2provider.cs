#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/MartianTile2.aspx")]
    public class MartianTile2Provider : HiRise
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public MartianTile2Provider(IPlateTilePyramid plateTiles, WwtOptions options)
        {
            _plateTiles = plateTiles;
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
            string dataset = values[3];

            switch (dataset)
            {
                case "mars_base_map":
                    if (level < 18)
                    {
                        using (Stream s = await _plateTiles.GetStreamAsync(@"\\wwt-mars\marsroot\MARSBASEMAP", "marsbasemap.plate", -1, level, tileX, tileY, token))
                        {
                            if (s == null || (int)s.Length == 0)
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
                    break;

                case "mars_hirise":
                    if (level < 19)
                    {
                        context.Response.ContentType = "image/png";

                        UInt32 index = ComputeHash(level, tileX, tileY) % 300;

                        using (Stream s = await _plateTiles.GetStreamAsync(@"\\wwt-mars\marsroot\hirise", $"hiriseV5_{index}.plate", -1, level, tileX, tileY, token))
                        {
                            if (s == null || (int)s.Length == 0)
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

                    break;

                case "mars_moc":
                    if (level < 18)
                    {
                        context.Response.ContentType = "image/png";

                        UInt32 index = ComputeHash(level, tileX, tileY) % 400;

                        using (Stream s = await _plateTiles.GetStreamAsync(@"\\wwt-mars\marsroot\moc", $"mocv5_{index}.plate", -1, level, tileX, tileY, token))
                        {
                            if (s == null || (int)s.Length == 0)
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
                    break;

                // old cases:
                // "mars_terrain_color" => id = "220581050";
                // "mars_historic_green" => id = "1194136815";
                // "mars_historic_schiaparelli" => id = "1113282550";
                // "mars_historic_lowell" => id = "675790761";
                // "mars_historic_antoniadi" => id = "1648157275";
                // "mars_historic_mec1" => id = "2141096698";
            }

            // This used to download from $"http://wwt.nasa.gov/wwt/p/{dataset}/{level}/{tileX}/{tileY}.png"
            // That URL is no longer available.
            await Report404Async(context, $"Mars dataset {dataset} unavailable", token);
        }
    }
}
