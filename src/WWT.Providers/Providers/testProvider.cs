using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace WWT.Providers
{
    public class testProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            String baseName = ConfigurationManager.AppSettings["WWTToursTourFileUNC"].ToLower();
            context.Response.Write(baseName);
        }
    }
}
