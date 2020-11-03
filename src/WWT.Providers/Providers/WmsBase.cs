using System;
using System.Threading;
using System.Threading.Tasks;
using WWT.Imaging;
using WWTWebservices;

namespace WWT.Providers
{
    public abstract class WmsBase : RequestProvider
    {
        private readonly IToastTileMapBuilder _toastTileMap;

        protected abstract ImageSource Source { get; }

        public WmsBase(IToastTileMapBuilder toastTileMap)
        {
            _toastTileMap = toastTileMap;
        }

        public sealed override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            bool debug = context.Request.Params["debug"] != null;

            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string wmsUrl = values[3];

            if (level > 15)
            {
                context.Response.Write("No image");
                context.Response.End();
            }
            else if (debug)
            {
                var result = _toastTileMap.GetToastTileMapAddress(wmsUrl, level, tileX, tileY, ImageSource.MarsAsu);

                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                context.Response.Write(result);
                context.Response.End();
            }
            else
            {
                using var stream = await _toastTileMap.CreateToastTileMapAsync(wmsUrl, level, tileX, tileY, ImageSource.MarsAsu, token);

                await stream.CopyToAsync(context.Response.OutputStream, token);
            }
        }
    }
}
