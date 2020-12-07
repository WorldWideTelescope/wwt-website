#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/dembath.aspx")]
    public class DembathProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Text;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            //string query = context.Request.Params["Q"];
            //string[] values = query.Split(',');
            //int level = Convert.ToInt32(values[0]);
            //int tileX = Convert.ToInt32(values[1]);
            //int tileY = Convert.ToInt32(values[2]);
            //string filename = $@"D:\DEM\bath\{level}\{tileX}\L{level}X{tileX}Y{tileY}.dem";
            return Report404Async(context, "no dembath tile", token);
        }
    }
}
