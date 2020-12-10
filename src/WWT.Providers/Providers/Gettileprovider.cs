#nullable disable

using System;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/GetTile.aspx")]
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
            (var errored, var level, var tileX, var tileY, var id) = await HandleLXYExtraQParameter(context, token);
            if (errored)
                return;

            using var stream = await _tileAccessor.GetTileAsync(id, level, tileX, tileY, token);

            if (stream is null)
            {
                context.Response.StatusCode = 404;
            }
            else
            {
                await stream.CopyToAsync(context.Response.OutputStream, token);
            }
        }
    }
}
