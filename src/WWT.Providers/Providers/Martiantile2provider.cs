using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
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
            string id = "nothing";

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
                                context.Response.Write("No image");
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
                case "mars_terrain_color":
                    id = "220581050";
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
                                context.Response.Write("No image");
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
                                context.Response.Write("No image");
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
                case "mars_historic_green":
                    id = "1194136815";
                    break;
                case "mars_historic_schiaparelli":
                    id = "1113282550";
                    break;
                case "mars_historic_lowell":
                    id = "675790761";
                    break;
                case "mars_historic_antoniadi":
                    id = "1648157275";
                    break;
                case "mars_historic_mec1":
                    id = "2141096698";
                    break;

            }

            string filename = $@"{_options.DssToastPng}\wwtcache\mars\{id}\{level}\{tileY}\{tileX}_{tileY}.png";

            if (!File.Exists(filename))
            {
                // This used to download from $"http://wwt.nasa.gov/wwt/p/{dataset}/{level}/{tileX}/{tileY}.png"
                // That URL is no longer available.
                context.Response.StatusCode = 404;
            }
            else
            {
                context.Response.WriteFile(filename);
            }
        }
    }
}
