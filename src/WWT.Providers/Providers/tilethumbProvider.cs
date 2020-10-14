using System.IO;

namespace WWT.Providers
{
    public class TilethumbProvider : RequestProvider
    {
        private readonly FilePathOptions _options;

        public TilethumbProvider(FilePathOptions options)
        {
            _options = options;
        }

        public override void Run(IWwtContext context)
        {
            string name = context.Request.Params["name"];
            string path = Path.Combine(_options.DSSTileCache, "imagesTiler", "thumbnails");
            string filename = Path.Combine(path, $"{name}.jpg");

            if (File.Exists(filename))
            {
                context.Response.WriteFile(filename);
                context.Response.End();
            }
        }
    }
}
