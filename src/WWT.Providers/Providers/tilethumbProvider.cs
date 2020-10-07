using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class tilethumbProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string name = context.Request.Params["name"];
            string type = context.Request.Params["class"];
            string path = ConfigurationManager.AppSettings["DSSTileCache"] + "\\imagesTiler\\thumbnails\\";

            string filename = path + name + ".jpg";
            if (File.Exists(filename))
            {
                context.Response.WriteFile(filename);
                context.Response.End();
            }
        }
    }
}
