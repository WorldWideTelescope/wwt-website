using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.Tours;

namespace WWT.Providers
{
    public class GetTourProvider : GetTourProviderBase
    {
        private readonly ITourAccessor _tourAccessor;

        public GetTourProvider(ITourAccessor tourAccessor)
        {
            _tourAccessor = tourAccessor;
        }

        public override string ContentType => ContentTypes.XWtt;

        protected override Task<Stream> GetStreamAsync(string id, CancellationToken token)
            => _tourAccessor.GetTourAsync(id, token);
    }
}
