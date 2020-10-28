using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class testfailoverProvider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.Write(ConfigurationManager.AppSettings["DSSTOASTPNG"]);
            return Task.CompletedTask;
        }
    }
}
