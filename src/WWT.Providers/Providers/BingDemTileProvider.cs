using Microsoft.Maps.ElevationAdjustmentService.HDPhoto;
using System;
using System.IO;
using System.Net;
using WWTWebservices;

namespace WWT.Providers
{
    public class BingDemTileProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            int demSize = 33 * 33;
            int parentL = Math.Max(1, level - 3);
            int DemGeneration = level - parentL;

            int count = (int)Math.Pow(2, 3 - DemGeneration);
            int tileSize = (int)Math.Pow(2, DemGeneration);

            int offsetX = (tileX % tileSize) * count * 32;
            int offsetY = (tileY % tileSize) * count * 32;

            var parentX = tileX / tileSize;
            var parentY = tileY / tileSize;

            string id = WWTUtil.GetTileID(parentX, parentY, parentL, false);
            int server = WWTUtil.GetServerID(parentX, parentY);
            WebClient client = new WebClient();

            string url = $"http://ecn.t{server}.tiles.virtualearth.net/tiles/d{id}.elv?g=1&n=z";

            byte[] data = client.DownloadData(url);
            MemoryStream stream = new MemoryStream(data);


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
                        DemData[indexI] = (float)tile.AltitudeInMeters(yh + offsetY, xh + offsetX);

                        xh += count;
                    }
                    yh += count;

                }
                data = new byte[DemData.Length * 4];
                MemoryStream ms = new MemoryStream(data);
                BinaryWriter bw = new BinaryWriter(ms);

                foreach (float sample in DemData)
                {
                    bw.Write(sample);
                }
                bw.Flush();
                context.Response.BinaryWrite(data);
            }

            context.Response.End();
        }
    }
}
