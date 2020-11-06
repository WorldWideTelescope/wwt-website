using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class ThumbnailProvider : RequestProvider
    {
        private readonly IThumbnailAccessor _thumbnails;

        public ThumbnailProvider(IThumbnailAccessor thumbnails)
        {
            _thumbnails = thumbnails;
        }

        public override string ContentType => ContentTypes.Jpeg;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string name = context.Request.Params["name"];
            string type = context.Request.Params["class"];

            using (var s = await GetThumbnailAsync(name, type, token))
            {
                await s.CopyToAsync(context.Response.OutputStream, token);
                context.Response.Flush();
                context.Response.End();
            }
        }

        private async Task<Stream> GetThumbnailAsync(string name, string type, CancellationToken token)
            => await _thumbnails.GetThumbnailStreamAsync(name, type, token).ConfigureAwait(false)
            ?? await _thumbnails.GetDefaultThumbnailStreamAsync(token).ConfigureAwait(false);
    }
}
