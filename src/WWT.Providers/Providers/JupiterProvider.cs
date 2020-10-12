using System;
using WWTWebservices;

namespace WWT.Providers
{
    public class JupiterProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public JupiterProvider(IPlateTilePyramid plateTiles, FilePathOptions options)
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

            if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (var s = _plateTiles.GetStream(_options.WwtTilesDir, "Jupiter.plate", level, tileX, tileY))
                {
                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
        }
    }
}
