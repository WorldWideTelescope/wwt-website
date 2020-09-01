using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class g360Provider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];

            context.Response.AddHeader("Cache-Control", "public, max-age=31536000");
            context.Response.AddHeader("Expires", "Thu, 31 Dec 2009 16:00:00 GMT");
            context.Response.AddHeader("ETag", "155");
            context.Response.AddHeader("Last-Modified", "Tue, 20 May 2008 22:32:37 GMT");
            //context.Response.AddHeader("Cache-Control", "max-age=36000");


            if (level < 12)
            {
                context.Response.ContentType = "image/png";
                UInt32 index = DirectoryEntry.ComputeHash(level + 128, tileX, tileY) % 16;
                Stream s = PlateFile2.GetFileStream(String.Format(wwtTilesDir + "\\g360-{0}.plate", index.ToString()), -1, level, tileX, tileY);
                if (s == null || s.Length == 0)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("No image");
                    context.Response.End();
                    return;
                }

                int length = (int)s.Length;
                byte[] data = new byte[length];
                s.Read(data, 0, length);
                s.Close();
                context.Response.OutputStream.Write(data, 0, length);
                context.Response.Flush();
                context.Response.End();
                return;
            }
        }
    }
}
