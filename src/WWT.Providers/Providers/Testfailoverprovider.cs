using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class TestfailoverProvider : RequestProvider
    {
        private readonly WwtOptions _options;

        public TestfailoverProvider(WwtOptions options)
        {
            _options = options;
        }

        public override string ContentType => ContentTypes.Html;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Write(_options.DssToastPng);
            return Task.CompletedTask;
        }
    }
}
