using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WWT.Providers
{
    public class testProvider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            String baseName = ConfigurationManager.AppSettings["WWTToursTourFileUNC"].ToLower();
            context.Response.Write(baseName);
            return Task.CompletedTask;
        }
    }
}
