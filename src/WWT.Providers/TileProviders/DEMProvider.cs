#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/dem.aspx")]
    public class DemProvider : RequestProvider
    {
        private readonly WwtOptions _options;

        public DemProvider(WwtOptions options)
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
                byte[] data = new byte[demSize];
                FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Seek((long)(demSize * tileX), SeekOrigin.Begin);
                fs.Read(data, 0, demSize);
                fs.Close();
                await context.Response.OutputStream.WriteAsync(data, 0, demSize, token);
            }

            context.Response.End();
        }
    }
}
