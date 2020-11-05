using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.Tours;

namespace WWT.Providers
{
    public class GetTourThumbnailProvider : GetTourProviderBase
    {
        private readonly ITourAccessor _tourAccessor;

        public override string ContentType => ContentTypes.Png;

        public GetTourThumbnailProvider(ITourAccessor tourAccessor)
        {
            _tourAccessor = tourAccessor;
        }

        protected override Task<Stream> GetStreamAsync(string id, CancellationToken token)
            => _tourAccessor.GetTourThumbnailAsync(id, token);
    }
}
