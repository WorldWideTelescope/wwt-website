using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;
using WWTWebservices;

namespace WWT.Providers
{
    public class Tiles2Provider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly IKnownPlateFiles _knownPlateFiles;
        private readonly FilePathOptions _options;

        public Tiles2Provider(IPlateTilePyramid plateTiles, IKnownPlateFiles knownPlateFiles, FilePathOptions options)
        {
            _plateTiles = plateTiles;
            _knownPlateFiles = knownPlateFiles;
            _options = options;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            context.Response.AddHeader("Cache-Control", "public, max-age=31536000");
            context.Response.AddHeader("Expires", "Thu, 31 Dec 2009 16:00:00 GMT");
            context.Response.AddHeader("ETag", "155");
            context.Response.AddHeader("Last-Modified", "Tue, 20 May 2008 22:32:37 GMT");

            if (_knownPlateFiles.TryNormalizePlateName(values[3], out var file) && level < 10)
            {
                context.Response.ContentType = "image/png";
                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, $"{file}.plate", -1, level, tileX, tileY, token))
                {
                    if (s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                    }
                    else
                    {
                        await s.CopyToAsync(context.Response.OutputStream, token);
                        context.Response.Flush();
                        context.Response.End();
                    }
                }
            }

            return;;
        }
    }
}
