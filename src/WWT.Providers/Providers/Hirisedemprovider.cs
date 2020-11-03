using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class HiriseDemProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public HiriseDemProvider(IPlateTilePyramid plateTiles, FilePathOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            string filename = $@"\\wwt-mars\marsroot\dem\Merged4\{level}\{tileX}\DL{level}X{tileX}Y{tileY}.dem";

            if (!File.Exists(filename))
            {
                context.Response.ContentType = "image/png";
                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, $"marsToastDem.plate", -1, level, tileX, tileY, token))
                {
                    if (s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                        return; 
                    }

                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return; 
                }
            }

            context.Response.WriteFile(filename);
        }
    }
}
