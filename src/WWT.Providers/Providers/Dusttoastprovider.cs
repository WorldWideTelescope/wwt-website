using System;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class DustToastProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public DustToastProvider(IPlateTilePyramid plateTiles, WwtOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (var s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "dust.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
        }
    }
}
