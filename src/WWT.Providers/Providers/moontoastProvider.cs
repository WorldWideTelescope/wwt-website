using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class moontoastProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"] + "\\LROWAC";

            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            int octsetlevel = level;
            string filename;

            if (level > 10)
            {
                context.Response.Write("No image");
                context.Response.Close();
                return;
            }

            if (level < 7)
            {
                context.Response.ContentType = "image/png";
                Stream s = PlateTilePyramid.GetFileStream(wwtTilesDir + "\\L0X0Y0.plate", level, tileX, tileY);
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
                int powLev5Diff = (int)Math.Pow(2, L - 3);
                int X32 = X / powLev5Diff;
                int Y32 = Y / powLev5Diff;
                filename = string.Format(wwtTilesDir + @"\L3x{0}y{1}.plate", X32, Y32);

                int L5 = L - 3;
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
