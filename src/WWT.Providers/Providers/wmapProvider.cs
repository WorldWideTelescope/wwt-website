using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class wmapProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string file = "WMAP";
            string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];


            if (level < 8)
            {
                context.Response.ContentType = "image/png";
                Stream s = PlateTilePyramid.GetFileStream(String.Format(wwtTilesDir + "\\{0}.plate", file), level, tileX, tileY);
                int length = (int)s.Length;
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
