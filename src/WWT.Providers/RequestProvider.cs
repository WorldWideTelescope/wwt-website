using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public abstract class RequestProvider
    {
        public abstract Task RunAsync(IWwtContext context, CancellationToken token);
    }
}
