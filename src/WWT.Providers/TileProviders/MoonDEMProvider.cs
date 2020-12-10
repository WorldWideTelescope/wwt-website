#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/moondem.aspx")]
    public class MoondemProvider : RequestProvider
    {
        private readonly WwtOptions _options;

        public MoondemProvider(WwtOptions options)
        {
            _options = options;
        }

        public override string ContentType => ContentTypes.OctetStream;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY, var type) = await HandleLXYExtraQParameter(context, token);
            if (errored)
                return;

            int demSize = 513 * 2;
            string wwtDemDir = _options.WWTDEMDir;
            string filename = String.Format(wwtDemDir + @"\toast\moon\Chunks\{0}\{1}.chunk", level, tileY);

            if (File.Exists(filename))
            {
                byte[] data = new byte[demSize];
                FileStream fs = File.OpenRead(filename);
                fs.Seek((long)(demSize * tileX), SeekOrigin.Begin);

                fs.Read(data, 0, demSize);
                fs.Close();
                await context.Response.OutputStream.WriteAsync(data, 0, demSize, token);
            }
            else
            {
                byte[] data = new byte[demSize];

                await context.Response.OutputStream.WriteAsync(data, 0, demSize, token);
            }

            context.Response.End();
        }
    }
}
