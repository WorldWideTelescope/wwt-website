using System.IO;

namespace WWT.Providers
{
    public class FixedAltitudeDemTileProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            //int level = Convert.ToInt32(values[0]);
            //int tileX = Convert.ToInt32(values[1]);
            //int tileY = Convert.ToInt32(values[2]);
            string alt = context.Request.Params["alt"];
            string proj = context.Request.Params["proj"];
            float altitude = float.Parse(alt);
            int demSize = 33 * 33;

            if (proj.ToLower().StartsWith("t"))
            {
                demSize = 17 * 17;
            }

            BinaryWriter bw = new BinaryWriter(context.Response.OutputStream);

            for (int i = 0; i < demSize; i++)
            {
                bw.Write(altitude);
            }

            bw = null;
            context.Response.OutputStream.Flush();

            context.Response.End();
        }
    }
}
