using System;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class DemMarsProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;

        public DemMarsProvider(IPlateTilePyramid plateTiles)
        {
            _plateTiles = plateTiles;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level < 18)
            {
                using (var s = _plateTiles.GetStream(@"\\wwtfiles.file.core.windows.net\wwtmars\MarsDem", "marsToastDem.plate", -1, level, tileX, tileY))
                {
                    if (s == null || (int)s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                    }
                    else
                    {
                        s.CopyTo(context.Response.OutputStream);
                        context.Response.Flush();
                        context.Response.End();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
