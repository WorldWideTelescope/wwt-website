using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/TwoMassToast.aspx")]
    public class TwoMassToastProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public TwoMassToastProvider(IPlateTilePyramid plateTiles, WwtOptions options)
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

            if (level > 7)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                context.Response.Write("No image");
                context.Response.End();
                return;;
            }
            else
            {
                try
                {
                    context.Response.ContentType = "image/png";

                    using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "2MassToast0to7.plate", level, tileX, tileY, token))
                    {
                        await s.CopyToAsync(context.Response.OutputStream, token);
                        context.Response.Flush();
                        context.Response.End();
                        return;;
                    }
                }
                catch
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("No image");
                    context.Response.End();
                    return;;
                }
            }
        }
    }
}
