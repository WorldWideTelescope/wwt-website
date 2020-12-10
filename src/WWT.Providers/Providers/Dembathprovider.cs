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
            //string filename = $@"D:\DEM\bath\{level}\{tileX}\L{level}X{tileX}Y{tileY}.dem";
            return Report404Async(context, "no dembath tile", token);
        }
    }
}
