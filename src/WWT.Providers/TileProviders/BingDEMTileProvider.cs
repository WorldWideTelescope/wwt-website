#nullable disable

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.Maps;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/BingDemTile.aspx")]
    public class BingDemTileProvider : RequestProvider
    {
        private readonly IVirtualEarthDownloader _veDownloader;
        private readonly ActivitySource _activitySource;

        public BingDemTileProvider(IVirtualEarthDownloader veDownloader, [FromKeyedServices(Constants.ActivitySourceName)] ActivitySource activitySource)
        {
            _veDownloader = veDownloader;
            _activitySource = activitySource;
        }

        public override string ContentType => ContentTypes.OctetStream;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            int demSize = 33 * 33;
            int parentL = Math.Max(1, level - 3);
            int DemGeneration = level - parentL;

            int count = (int)Math.Pow(2, 3 - DemGeneration);
            int tileSize = (int)Math.Pow(2, DemGeneration);

            int offsetX = (tileX % tileSize) * count * 32;
            int offsetY = (tileY % tileSize) * count * 32;

            var parentX = tileX / tileSize;
            var parentY = tileY / tileSize;

            using var stream = await _veDownloader.DownloadVeTileAsync(VirtualEarthTile.Ecn, parentL, parentX, parentY, token);

            using var activity = _activitySource.CreateActivity("BingDemTileProvider", ActivityKind.Internal)?.Start();

            var tile = await DemCodec.DecompressAsync(stream, token);

            if (tile != null)
            {
                float[] DemData = new float[demSize];
                int yh = 0;
                for (int yl = 0; yl < 33; yl++)
                {
                    int xh = 0;
                    for (int xl = 0; xl < 33; xl++)
                    {
                        int indexI = xl + (32 - yl) * 33;
                        DemData[indexI] = (float)tile.AltitudeInMeters(yh + offsetY, xh + offsetX);

                        xh += count;
                    }
                    yh += count;

                }

                var data = new byte[DemData.Length * 4];
                using var ms = new MemoryStream(data);

                var bw = new BinaryWriter(ms);

                foreach (float sample in DemData)
                {
                    bw.Write(sample);
                }
                bw.Flush();
                await context.Response.OutputStream.WriteAsync(data, 0, data.Length, token);
            }

            context.Response.End();
        }
    }
}
