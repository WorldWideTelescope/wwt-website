using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class TilethumbProvider : RequestProvider
    {
        private readonly FilePathOptions _options;

        public TilethumbProvider(FilePathOptions options)
        {
            _options = options;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string name = context.Request.Params["name"];
            string path = Path.Combine(_options.DSSTileCache, "imagesTiler", "thumbnails");
            string filename = Path.Combine(path, $"{name}.jpg");

            if (File.Exists(filename))
            {
                context.Response.WriteFile(filename);
                context.Response.End();
            }

            return Task.CompletedTask;
        }
    }
}
