using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class XML2WTTProvider : WWTWeb_XML2WTT
    {
        private readonly WwtOptions _options;

        public XML2WTTProvider(IFileNameHasher hasher, WwtOptions options)
            : base(hasher)
        {
            _options = options;
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string tourcache = _options.WwtTourCache;

            context.Response.ClearHeaders();
            context.Response.Clear();
            context.Response.ContentType = "application/x-wtt";

            context.Response.WriteFile(MakeTourFromXML(context, context.Request.InputStream, tourcache + "\\temp\\"));

            return Task.CompletedTask;
        }
    }
}
