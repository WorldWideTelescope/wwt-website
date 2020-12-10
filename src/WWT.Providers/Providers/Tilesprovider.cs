#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/tiles.aspx")]
    public class TilesProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly IKnownPlateFiles _knownPlateFiles;
        private readonly WwtOptions _options;

        public TilesProvider(IPlateTilePyramid plateTiles, IKnownPlateFiles knownPlateFiles, WwtOptions options)
        {
            _plateTiles = plateTiles;
            _knownPlateFiles = knownPlateFiles;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY, var dataset) = await HandleLXYExtraQParameter(context, token);
            if (errored)
                return;

            if (_knownPlateFiles.TryNormalizePlateName(dataset, out var file) && level < 8)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, $"{file}.plate", level, tileX, tileY, token))
                {
                    if (s.Length == 0)
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

            return;;
        }
    }
}
