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
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

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
