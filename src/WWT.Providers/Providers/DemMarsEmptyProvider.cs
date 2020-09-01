using System;
using System.IO;
using System.Net;
using WWTWebservices;

namespace WWT.Providers
{
    public class DemMarsEmptyProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            //string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
            string DSSTileCache = WWTUtil.GetCurrentConfigShare("DSSTileCache", true);

            string filename = String.Format(DSSTileCache + "\\wwtcache\\mars\\dem\\{0}\\{2}\\{1}_{2}.dem", level, tileX, tileY);
            string path = String.Format(DSSTileCache + "\\wwtcache\\mars\\dem\\{0}\\{2}", level, tileX, tileY);



            if (!File.Exists(filename))
            {
                try
                {
                    if (!Directory.Exists(filename))
                    {
                        Directory.CreateDirectory(path);
                    }

                    WebClient webclient = new WebClient();

                    string url = string.Format("http://wwt.nasa.gov/wwt/p/mars_toast_dem_32f/{0}/{1}/{2}.toast_dem_v1", level, tileX, tileY);

                    webclient.DownloadFile(url, filename);
                }
                catch
                {
                    context.Response.StatusCode = 404;
                    return;
                }
            }

            context.Response.Write("ok");

        }
    }
}
