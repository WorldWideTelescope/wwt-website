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

            string filename = $@"D:\DEM\bath\{level}\{tileX}\L{level}X{tileX}Y{tileY}.dem";

            if (!File.Exists(filename))
            {
                context.Response.StatusCode = 404;
            }
            else
            {
                context.Response.WriteFile(filename);
            }
        }
    }
}
