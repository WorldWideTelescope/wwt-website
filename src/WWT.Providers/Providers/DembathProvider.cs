using System;
using System.IO;

namespace WWT.Providers
{
    public class DembathProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            string filename = String.Format("D:\\DEM\\bath\\{0}\\{1}\\L{0}X{1}Y{2}.dem", level, tileX, tileY);

            if (!File.Exists(filename))
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Response.WriteFile(filename);
        }
    }
}
