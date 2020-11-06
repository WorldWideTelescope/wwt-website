using System;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class GetTileProvider : RequestProvider
    {
        private readonly ITileAccessor _tileAccessor;

        public GetTileProvider(ITileAccessor tileAccessor)
        {
            _tileAccessor = tileAccessor;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string dataset = values[3];
            string id = dataset;

            using var stream = await _tileAccessor.GetTileAsync(id, level, tileX, tileY, token);

            if (stream is null)
            {
                context.Response.StatusCode = 404;
            }
            else
            {
                await stream.CopyToAsync(context.Response.OutputStream);
            }
        }
    }
}
