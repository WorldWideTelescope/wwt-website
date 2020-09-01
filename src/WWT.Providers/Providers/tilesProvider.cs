using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class tilesProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string file = values[3];
            string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];

            context.Response.AddHeader("Cache-Control", "public, max-age=31536000");
            context.Response.AddHeader("Expires", "Thu, 31 Dec 2009 16:00:00 GMT");
            context.Response.AddHeader("ETag", "155");
            context.Response.AddHeader("Last-Modified", "Tue, 20 May 2008 22:32:37 GMT");
            //context.Response.AddHeader("Cache-Control", "max-age=36000");


            if (level < 8)
            {
                context.Response.ContentType = "image/png";
                Stream s = PlateTilePyramid.GetFileStream(String.Format(wwtTilesDir + "\\{0}.plate", file), level, tileX, tileY);
                int length = (int)s.Length;
                if (length == 0)
                {

                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("No image");
                    context.Response.End();
                    return;
                }
                byte[] data = new byte[length];
                s.Read(data, 0, length);
                context.Response.OutputStream.Write(data, 0, length);
                context.Response.Flush();
                context.Response.End();
                return;
            }
        }
    }
}
