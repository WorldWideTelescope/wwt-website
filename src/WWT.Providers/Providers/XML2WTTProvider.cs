using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class XML2WTTProvider : WWTWeb_XML2WTT
    {
        public XML2WTTProvider(IFileNameHasher hasher)
            : base(hasher)
        {
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string tourcache = ConfigurationManager.AppSettings["WWTTOURCACHE"];

            context.Response.ClearHeaders();
            context.Response.Clear();
            context.Response.ContentType = "application/x-wtt";

            context.Response.WriteFile(MakeTourFromXML(context, context.Request.InputStream, tourcache + "\\temp\\"));

            return Task.CompletedTask;
        }
    }
}
