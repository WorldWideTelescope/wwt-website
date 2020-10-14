using System;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class TilesProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public TilesProvider(IPlateTilePyramid plateTiles, FilePathOptions options)
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
            string file = values[3];
            string wwtTilesDir = _options.WwtTilesDir;

            context.Response.AddHeader("Cache-Control", "public, max-age=31536000");
            context.Response.AddHeader("Expires", "Thu, 31 Dec 2009 16:00:00 GMT");
            context.Response.AddHeader("ETag", "155");
            context.Response.AddHeader("Last-Modified", "Tue, 20 May 2008 22:32:37 GMT");

            if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (Stream s =_plateTiles.GetStream(wwtTilesDir, $"{file}.plate", level, tileX, tileY))
                {
                    if (s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                    }
                    else
                    {
                        s.CopyTo(context.Response.OutputStream);
                        context.Response.Flush();
                        context.Response.End();
                    }
                }
            }
        }
    }
}
