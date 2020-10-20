using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class DemMarsEmptyProvider : RequestProvider
    {
        private readonly FilePathOptions _options;

        public DemMarsEmptyProvider(FilePathOptions options)
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

            string path = Path.Combine(_options.DSSTileCache, "wwtcache", "mars", "dem", level.ToString(), tileY.ToString());
            string filename = Path.Combine(path, $"{tileX}_{tileY}.dem");

            if (!File.Exists(filename))
            {
                try
                {
                    if (!Directory.Exists(filename))
                    {
                        Directory.CreateDirectory(path);
                    }

                    WebClient webclient = new WebClient();

                    string url = $"http://wwt.nasa.gov/wwt/p/mars_toast_dem_32f/{level}/{tileX}/{tileY}.toast_dem_v1";

                    webclient.DownloadFile(url, filename);
                }
                catch
                {
                    context.Response.StatusCode = 404;
                    return Task.CompletedTask;
                }
            }

            context.Response.Write("ok");

            return Task.CompletedTask;
        }
    }
}
