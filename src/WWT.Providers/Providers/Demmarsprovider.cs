#nullable disable

using System;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/demmars.aspx")]
    public class DemMarsProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;

        public DemMarsProvider(IPlateTilePyramid plateTiles)
        {
            _plateTiles = plateTiles;
        }

        public override string ContentType => ContentTypes.OctetStream;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            if (level < 18)
            {
                using (var s = await _plateTiles.GetStreamAsync(@"\\wwtfiles.file.core.windows.net\wwtmars\MarsDem", "marsToastDem.plate", -1, level, tileX, tileY, token))
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
    }
}
