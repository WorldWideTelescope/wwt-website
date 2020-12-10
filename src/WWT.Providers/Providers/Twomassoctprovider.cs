#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/2MASSOct.aspx")]
    public class TwoMASSOctProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public TwoMASSOctProvider(IPlateTilePyramid plateTiles, WwtOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "2massoctset.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                }
            }

            return;;
        }
    }
}
