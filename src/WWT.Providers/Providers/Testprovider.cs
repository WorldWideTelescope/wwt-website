using System;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class TestProvider : RequestProvider
    {
        private readonly WwtOptions _options;

        public TestProvider(WwtOptions options)
        {
            _options = options;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            String baseName = _options.WwtToursTourFileUNC.ToLower();
            context.Response.Write(baseName);
            return Task.CompletedTask;
        }
    }
}
