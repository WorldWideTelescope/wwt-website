using System;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class MipsgalProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public MipsgalProvider(IPlateTilePyramid plateTiles, FilePathOptions options)
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

            if (level > 11)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                context.Response.Write("No image");
                context.Response.End();
                return;
            }

            if (level < 11)
            {
                try
                {
                    context.Response.ContentType = "image/png";
                    using (Stream s = _plateTiles.GetStream(_options.WwtTilesDir, "mipsgal_L0to10_x0_y0.plate", level, tileX, tileY))
                    {
                        s.CopyTo(context.Response.OutputStream);
                        context.Response.Flush();
                        context.Response.End();
                        return;
                    }
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
                    int powLev3Diff = (int)Math.Pow(2, L - 1);
                    int X8 = X / powLev3Diff;
                    int Y8 = Y / powLev3Diff;

                    int L3 = L - 1;
                    int X3 = X % powLev3Diff;
                    int Y3 = Y % powLev3Diff;
                    context.Response.ContentType = "image/png";

                    using (Stream s = _plateTiles.GetStream(_options.WwtTilesDir, $"mipsgal_L1to11_x{X8}_y{Y8}.plate", L3, X3, Y3))
                    {
                        s.CopyTo(context.Response.OutputStream);
                        context.Response.Flush();
                        context.Response.End();
                        return;
                    }
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
        }
    }
}
