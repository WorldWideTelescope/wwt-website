using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class GalexToastProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
            string wwtgalexdir = WWTUtil.GetCurrentConfigShare("WWTGALEXDIR", true);


            if (level > 10)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                context.Response.Write("No image");
                context.Response.End();
                return;
            }

            if (level < 9)
            {
                try
                {
                    context.Response.ContentType = "image/png";
                    Stream s = PlateTilePyramid.GetFileStream(wwtTilesDir + "\\GalexBoth_L0to8_x0_y0.plate", level, tileX, tileY);
                    int length = (int)s.Length;
                    byte[] data = new byte[length];
                    s.Read(data, 0, length);
                    context.Response.OutputStream.Write(data, 0, length);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
                catch
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("No image");
                    context.Response.End();
                    return;
                }

            }
            else
            {
                try
                {
                    int L = level;
                    int X = tileX;
                    int Y = tileY;
                    int powLev3Diff = (int)Math.Pow(2, L - 3);
                    int X8 = X / powLev3Diff;
                    int Y8 = Y / powLev3Diff;
                    var filename = string.Format(wwtgalexdir + @"\GalexBoth_L3to10_x{0}_y{1}.plate", X8, Y8);

                    int L3 = L - 3;
                    int X3 = X % powLev3Diff;
                    int Y3 = Y % powLev3Diff;
                    context.Response.ContentType = "image/png";
                    Stream s = PlateTilePyramid.GetFileStream(filename, L3, X3, Y3);
                    int length = (int)s.Length;
                    byte[] data = new byte[length];
                    s.Read(data, 0, length);
                    context.Response.OutputStream.Write(data, 0, length);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
                catch
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("No image");
                    context.Response.End();
                    return;
                }

            }

            // This file has returns which cause this warning to show in the generated files.
            // This should be refactored, but that will be a bigger change.
#pragma warning disable 0162
        }
    }
}
