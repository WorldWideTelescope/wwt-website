using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class TilethumbProvider : RequestProvider
    {
        private readonly ITileAccessor _tileAccessor;

        public TilethumbProvider(ITileAccessor tileAccessor)
        {
            _tileAccessor = tileAccessor;
        }

        public override string ContentType => ContentTypes.Jpeg;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string name = context.Request.Params["name"];

            using var stream = await _tileAccessor.GetThumbnailAsync(name, token);

            if (stream != null)
            {
                await stream.CopyToAsync(context.Response.OutputStream, token);
            }
        }
    }
}
