#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/marsdem.aspx")]
    public class MarsdemProvider : RequestProvider
    {
        private readonly WwtOptions _options;

        public MarsdemProvider(WwtOptions options)
        {
            _options = options;
        }

        public override string ContentType => ContentTypes.OctetStream;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            // These files seem to have been lost. Azure behavior is to return all zeros.
            //string filename = $@"{_options.WWTDEMDir}\toast\mars\Chunks\{level}\{tileY}.chunk";

            int demSize = 513 * 2;
            byte[] data = new byte[demSize];
            await context.Response.OutputStream.WriteAsync(data, 0, demSize, token);
            context.Response.End();
        }
    }
}
