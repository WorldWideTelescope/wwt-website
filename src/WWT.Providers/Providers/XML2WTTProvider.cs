using System.Configuration;
using WWTWebservices;

namespace WWT.Providers
{
    public class XML2WTTProvider : WWTWeb_XML2WTT
    {
        public XML2WTTProvider(IFileNameHasher hasher)
            : base(hasher)
        {
        }

        public override void Run(IWwtContext context)
        {
            //string etag = context.Request.Headers["If-None-Match"];
            string tourcache = ConfigurationManager.AppSettings["WWTTOURCACHE"];

            context.Response.ClearHeaders();
            context.Response.Clear();
            context.Response.ContentType = "application/x-wtt";

            context.Response.WriteFile(MakeTourFromXML(context, context.Request.InputStream, tourcache + "\\temp\\"));

            //context.Response.OutputStream
        }
    }
}
