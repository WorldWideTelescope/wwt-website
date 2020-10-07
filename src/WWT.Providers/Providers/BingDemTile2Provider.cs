using Microsoft.Maps.ElevationAdjustmentService.HDPhoto;
using System;
using System.IO;
using System.Net;
using WWTWebservices;

namespace WWT.Providers
{
    public class BingDemTile2Provider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            const int demSize = 33 * 33;

            string id = WWTUtil.GetTileID(tileX, tileY, level, false);
            int server = WWTUtil.GetServerID(tileX, tileY);
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
                        DemData[indexI] = (float)tile.AltitudeInMeters(yh, xh);

                        xh += 8;
                    }
                    yh += 8;

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
