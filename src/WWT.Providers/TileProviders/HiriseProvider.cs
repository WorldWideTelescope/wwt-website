#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/Hirise.aspx")]
    public class HiriseProvider : HiRise
    {
        private readonly IPlateTilePyramid _plateTiles;

        public HiriseProvider(IPlateTilePyramid plateTiles)
        {
            _plateTiles = plateTiles;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            if (level < 19)
            {
                context.Response.ContentType = "image/png";

                UInt32 index = ComputeHash(level, tileX, tileY) % 300;

                using (Stream s = await _plateTiles.GetStreamAsync(@"\\wwt-mars\marsroot\hirise", $"hiriseV5_{index}.plate", -1, level, tileX, tileY, token))
                {
                    if (s == null || (int)s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("No image", token);
                        context.Response.End();
                        return;
                    }

                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }
        }
    }
}
