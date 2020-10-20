using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class GetTileProvider : RequestProvider
    {
        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string dataset = values[3];
            string id = dataset;

            string DSSTileCache = ConfigurationManager.AppSettings["DSSTileCache"];

            string filename = String.Format(DSSTileCache + "\\imagesTiler\\{3}\\{0}\\{2}\\{2}_{1}.png", level, tileX, tileY, id);

            if (!File.Exists(filename))
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            context.Response.WriteFile(filename);
            return Task.CompletedTask;
        }
    }
}
