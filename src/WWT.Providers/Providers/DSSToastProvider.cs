using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class DSSToastProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
            string dsstoastpng = WWTUtil.GetCurrentConfigShare("DSSTOASTPNG", true);

            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            int octsetlevel = level;
            string filename;

            if (level > 12)
            {
                context.Response.Write("No image");
                context.Response.Close();
                return;
            }

            if (level < 8)
            {
                context.Response.ContentType = "image/png";
                Stream s = PlateTilePyramid.GetFileStream(wwtTilesDir + "\\dsstoast.plate", level, tileX, tileY);
                int length = (int)s.Length;
                byte[] data = new byte[length];
                s.Read(data, 0, length);
                context.Response.OutputStream.Write(data, 0, length);
                context.Response.Flush();
                context.Response.End();
                return;
            }
            else
            {
                int L = level;
                int X = tileX;
                int Y = tileY;
                string mime = "png";
                int powLev5Diff = (int)Math.Pow(2, L - 5);
                int X32 = X / powLev5Diff;
                int Y32 = Y / powLev5Diff;
                filename = string.Format(dsstoastpng + @"\DSS{0}L5to12_x{1}_y{2}.plate", mime, X32, Y32);

                int L5 = L - 5;
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

            // This file has returns which cause this warning to show in the generated files.
            // This should be refactored, but that will be a bigger change.
#pragma warning disable 0162
        }
    }
}
