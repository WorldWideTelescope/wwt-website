using System;
using System.Configuration;
using System.IO;

namespace WWT.Providers
{
    public class DemProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            //string type = values[3];
            int demSize = 33 * 33 * 2;

            string wwtDemDir = ConfigurationManager.AppSettings["WWTDEMDir"];
            string filename = String.Format(wwtDemDir + @"\Mercator\Chunks\{0}\{1}.chunk", level, tileY);

            if (File.Exists(filename))
            {

                byte[] data = new byte[demSize];
                FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Seek((long)(demSize * tileX), SeekOrigin.Begin);
                fs.Read(data, 0, demSize);
                fs.Close();
                context.Response.OutputStream.Write(data, 0, demSize);
                context.Response.OutputStream.Flush();

            }
            context.Response.End();
        }
    }
}
