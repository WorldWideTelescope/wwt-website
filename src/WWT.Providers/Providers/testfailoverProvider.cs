using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class testfailoverProvider : RequestProvider
    {
        private readonly FilePathOptions _options;

        public testfailoverProvider(FilePathOptions options)
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
