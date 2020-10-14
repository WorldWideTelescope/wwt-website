using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class EarthMerBathProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public EarthMerBathProvider(IPlateTilePyramid plateTiles, FilePathOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override void Run(IWwtContext context)
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
                return;
            }

            if (level < 8)
            {
                context.Response.ContentType = "image/png";
                using (Stream s = _plateTiles.GetStream(_options.WwtTilesDir, "BmngMerBase.plate", level, tileX, tileY))
                {
                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                    return;
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
                using (Stream s = _plateTiles.GetStream(_options.WwtTilesDir, $"BmngMerL2X{X32}Y{Y32}.plate", L5, X5, Y5))
                {
                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }

            System.Net.WebClient client = new System.Net.WebClient();

            string url = String.Format("http://a{0}.ortho.tiles.virtualearth.net/tiles/a{1}.jpeg?g=15", WWTUtil.GetServerID(tileX, tileY), WWTUtil.GetTileID(tileX, tileY, level, false));

            byte[] dat = client.DownloadData(url);

            client.Dispose();

            context.Response.OutputStream.Write(dat, 0, dat.Length);
        }
    }
}
