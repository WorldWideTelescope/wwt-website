using System;
using System.Configuration;
using WWTWebservices;

namespace WWT.Providers
{
    public class DSSProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTile;

        public DSSProvider(IPlateTilePyramid plateTile)
        {
            _plateTile = plateTile;
        }

        public override void Run(WwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level > 12)
            {
                context.Response.Write("No image");
                context.Response.Close();
            }
            else if (level < 8)
            {
                string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];

                context.Response.ContentType = "image/png";

                using (var s = _plateTile.GetStream(wwtTilesDir, "dssterrapixel.plate", level, tileX, tileY))
                {
                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            else
            {
                int powLev5Diff = (int)Math.Pow(2, level - 5);
                int X32 = tileX / powLev5Diff;
                int Y32 = tileY / powLev5Diff;

                int L5 = level - 5;
                int X5 = tileX % powLev5Diff;
                int Y5 = tileY % powLev5Diff;

                context.Response.ContentType = "image/png";

                string dssTerapixelDir = ConfigurationManager.AppSettings["DssTerapixelDir"];
                string filename = $"DSSpngL5to12_x{X32}_y{Y32}.plate";

                using (var s = _plateTile.GetStream(dssTerapixelDir, filename, L5, X5, Y5))
                {
                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
        }
    }
}
