using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class EarthMerBathProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
            string dsstoastpng = ConfigurationManager.AppSettings["DSSTOASTPNG"];

            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            int octsetlevel = level;
            string filename;

            if (level > 20)
            {
                context.Response.Write("No image");
                context.Response.Close();
                return;
            }

            if (level < 8)
            {
                context.Response.ContentType = "image/png";
                Stream s = PlateTilePyramid.GetFileStream(wwtTilesDir + "\\BmngMerBase.plate", level, tileX, tileY);
                int length = (int)s.Length;
                byte[] data = new byte[length];
                s.Read(data, 0, length);
                context.Response.OutputStream.Write(data, 0, length);
                context.Response.Flush();
                context.Response.End();
                return;
            }
            else if (level < 10)
            {
                int L = level;
                int X = tileX;
                int Y = tileY;
                string mime = "png";
                int powLev5Diff = (int)Math.Pow(2, L - 2);
                int X32 = X / powLev5Diff;
                int Y32 = Y / powLev5Diff;
                filename = string.Format(wwtTilesDir + @"\BmngMerL2X{1}Y{2}.plate", mime, X32, Y32);

                int L5 = L - 2;
                int X5 = X % powLev5Diff;
                int Y5 = Y % powLev5Diff;
                context.Response.ContentType = "image/png";
                Stream s = PlateTilePyramid.GetFileStream(filename, L5, X5, Y5);
                int length = (int)s.Length;
                byte[] data = new byte[length];
                s.Read(data, 0, length);
                context.Response.OutputStream.Write(data, 0, length);
                context.Response.Flush();
                context.Response.End();
                return;

            }

            System.Net.WebClient client = new System.Net.WebClient();


            string url = String.Format("http://a{0}.ortho.tiles.virtualearth.net/tiles/a{1}.jpeg?g=15", WWTUtil.GetServerID(tileX, tileY), WWTUtil.GetTileID(tileX, tileY, level, false));

            byte[] dat = client.DownloadData(url);


            client.Dispose();

            context.Response.OutputStream.Write(dat, 0, dat.Length);
        }
    }
}
