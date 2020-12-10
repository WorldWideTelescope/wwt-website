#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/FixedAltitudeDemTile.aspx")]
    public class FixedAltitudeDemTileProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.OctetStream;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            // You can give this endpoint a Q=L,X,Y parameter, but it doesn't actually
            // do anything with those values.

            string alt = context.Request.Params["alt"];
            string proj = context.Request.Params["proj"];
            float altitude = float.Parse(alt);
            int demSize = 33 * 33;

            if (proj.ToLower().StartsWith("t"))
            {
                demSize = 17 * 17;
            }

            var data = new byte[demSize * 4];
            using var ms = new MemoryStream(data);
            var bw = new BinaryWriter(ms);

            for (int i = 0; i < demSize; i++)
            {
                bw.Write(altitude);
            }

            bw.Flush();
            await context.Response.OutputStream.WriteAsync(data, 0, data.Length, token);
            context.Response.End();
        }
    }
}
