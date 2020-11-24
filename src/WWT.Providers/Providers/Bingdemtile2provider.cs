#nullable disable

using Microsoft.Maps.ElevationAdjustmentService.HDPhoto;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/BingDemTile2.aspx")]
    public class BingDemTile2Provider : RequestProvider
    {
        private readonly IVirtualEarthDownloader _veDownloader;

        public BingDemTile2Provider(IVirtualEarthDownloader veDownload)
        {
            _veDownloader = veDownload;
        }

        public override string ContentType => ContentTypes.OctetStream;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            const int demSize = 33 * 33;

            using var stream = await _veDownloader.DownloadVeTileAsync(VirtualEarthTile.Ecn, level, tileX, tileY, token);
            DemTile tile = DemCodec.Decompress(stream);

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
                        DemData[indexI] = (float)tile.AltitudeInMeters(yh, xh);

                        xh += 8;
                    }
                    yh += 8;

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
