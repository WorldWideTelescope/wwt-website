using System.Configuration;

namespace WWT.Providers
{
    public class XML2WTTProvider : WWTWeb_XML2WTT
    {
        public override void Run(WwtContext context)
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
