using System;
using System.Configuration;
using System.IO;

namespace WWT.Providers
{
    public class moondemProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string type = values[3];
            int demSize = 513 * 2;
            string wwtDemDir = ConfigurationManager.AppSettings["WWTDEMDir"];
            string filename = String.Format(wwtDemDir + @"\toast\moon\Chunks\{0}\{1}.chunk", level, tileY);

            if (File.Exists(filename))
            {

                byte[] data = new byte[demSize];
                FileStream fs = File.OpenRead(filename);
                fs.Seek((long)(demSize * tileX), SeekOrigin.Begin);

                fs.Read(data, 0, demSize);
                fs.Close();
                context.Response.OutputStream.Write(data, 0, demSize);
                context.Response.OutputStream.Flush();


            }
            else
            {

                byte[] data = new byte[demSize];

                context.Response.OutputStream.Write(data, 0, demSize);
                context.Response.OutputStream.Flush();


            }

            context.Response.End();
        }
    }
}
