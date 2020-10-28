using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class MarsdemProvider : RequestProvider
    {
        private readonly FilePathOptions _options;

        public MarsdemProvider(FilePathOptions options)
        {
            _options = options;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            int demSize = 513 * 2;

            string filename = $@"{_options.WWTDEMDir}\toast\mars\Chunks\{level}\{tileY}.chunk";

            if (File.Exists(filename))
            {
                byte[] data = new byte[demSize];
                FileStream fs = File.OpenRead(filename);
                fs.Seek((long)(demSize * tileX), SeekOrigin.Begin);

                fs.Read(data, 0, demSize);
                fs.Close();
                context.Response.OutputStream.Write(data, 0, demSize);
                context.Response.OutputStream.Flush();
            }
            else
            {
                byte[] data = new byte[demSize];

                context.Response.OutputStream.Write(data, 0, demSize);
                context.Response.OutputStream.Flush();
            }

            context.Response.End();

            return Task.CompletedTask;
        }
    }
}
