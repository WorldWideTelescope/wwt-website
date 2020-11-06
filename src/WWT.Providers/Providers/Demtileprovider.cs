using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class DemTileProvider : RequestProvider
    {
        private readonly WwtOptions _options;

        public DemTileProvider(WwtOptions options)
        {
            _options = options;
        }

        public override string ContentType => ContentTypes.OctetStream;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            const int demSize = 33 * 33 * 2;

            string filename = Path.Combine(_options.WWTDEMDir, "Mercator", "Chunks", level.ToString(), $"{tileY}.chunk");

            if (File.Exists(filename))
            {
                byte[] data = new byte[demSize];
                FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Seek((long)(demSize * tileX), SeekOrigin.Begin);
                fs.Read(data, 0, demSize);
                fs.Close();
                context.Response.OutputStream.Write(data, 0, demSize);
                context.Response.OutputStream.Flush();
            }

            context.Response.End();

            return Task.CompletedTask;
        }
    }
}
