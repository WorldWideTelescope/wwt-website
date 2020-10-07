using System.Configuration;
using WWTWebservices;

namespace WWT.Providers
{
    public class testfailoverProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            context.Response.Write(ConfigurationManager.AppSettings["DSSTOASTPNG"]);
        }
    }
}
