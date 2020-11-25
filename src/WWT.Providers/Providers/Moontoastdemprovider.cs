#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/moontoastdem.aspx")]
    public class MoontoastdemProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public MoontoastdemProvider(IPlateTilePyramid plateTiles, WwtOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override string ContentType => ContentTypes.OctetStream;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string wwtDemDir = Path.Combine(_options.WWTDEMDir, "toast", "lola");

            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level > 10)
            {
                await context.Response.WriteAsync("No image", token);
                context.Response.Close();
                return; ;
            }

            if (level < 7)
            {
                context.Response.ContentType = "application/octet-stream";
                using (Stream s = await _plateTiles.GetStreamAsync(wwtDemDir, "moontoast_L0X0Y0.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return; ;
                }
            }
            else
            {
                int powLev5Diff = (int)Math.Pow(2, level - 3);
                int X32 = tileX / powLev5Diff;
                int Y32 = tileY / powLev5Diff;

                int L5 = level - 3;
                int X5 = tileX % powLev5Diff;
                int Y5 = tileY % powLev5Diff;

                context.Response.ContentType = "application/octet-stream";

                using (Stream s = await _plateTiles.GetStreamAsync(wwtDemDir, $"moontoast_L3X{X32}Y{Y32}.plate", L5, X5, Y5, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return; ;
                }
            }
        }
    }
}
