using System;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/test.aspx")]
    public class TestProvider : RequestProvider
    {
        private readonly WwtOptions _options;

        public TestProvider(WwtOptions options)
        {
            _options = options;
        }

        public override string ContentType => ContentTypes.Html;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            String baseName = _options.WwtToursTourFileUNC.ToLower();
            return context.Response.WriteAsync(baseName, token);
        }
    }
}
