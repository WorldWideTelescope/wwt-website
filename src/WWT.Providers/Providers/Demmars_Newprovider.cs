#nullable disable

using System;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/demmars_new.aspx")]
    public class DemMarsNewProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTile;

        public DemMarsNewProvider(IPlateTilePyramid plateTile)
        {
            _plateTile = plateTile;
        }

        public override string ContentType => ContentTypes.OctetStream;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level < 18)
            {
                var index = ComputeHash(level, tileX, tileY) % 400;

                using (var s = await _plateTile.GetStreamAsync(@"\\wwt-mars\marsroot\dem\", $"marsToastDem_{index}.plate", -1, level, tileX, tileY, token))
                {
                    if (s == null || (int)s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("No image", token);
                        context.Response.End();
                    }
                    else
                    {
                        await s.CopyToAsync(context.Response.OutputStream, token);
                        context.Response.Flush();
                        context.Response.End();
                    }
                }
            }
        }

        private uint ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }
    }
}
