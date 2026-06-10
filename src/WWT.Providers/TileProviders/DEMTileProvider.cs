#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/demTile.aspx")]
    public class DemTileProvider : RequestProvider
    {
        private readonly WwtOptions _options;

        public DemTileProvider(WwtOptions options)
        {
            _options = options;
        }

        public override string ContentType => ContentTypes.OctetStream;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            const int demSize = 33 * 33 * 2;

            string filename = Path.Combine(_options.WWTDEMDir, "Mercator", "Chunks", level.ToString(), $"{tileY}.chunk");

            if (File.Exists(filename))
            {
                using var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var slice = StreamSlice.Create(fs, demSize * tileX, demSize);

                await slice.CopyToAsync(context.Response.OutputStream, token);
            }

            context.Response.End();
        }
    }
}
