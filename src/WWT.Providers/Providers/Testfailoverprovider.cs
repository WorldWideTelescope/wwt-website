using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class TestfailoverProvider : RequestProvider
    {
        private readonly FilePathOptions _options;

        public TestfailoverProvider(FilePathOptions options)
        {
            _options = options;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Write(_options.DssToastPng);
            return Task.CompletedTask;
        }
    }
}
