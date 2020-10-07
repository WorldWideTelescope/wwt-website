using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class HiriseDemProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            string file = "marsToastDem";
            string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
            string DSSTileCache = ConfigurationManager.AppSettings["DSSTileCache"];

            DSSTileCache = @"\\wwt-mars\marsroot";

            string filename = String.Format(DSSTileCache + "\\dem\\Merged4\\{0}\\{1}\\DL{0}X{1}Y{2}.dem", level, tileX, tileY);

            string path = String.Format(DSSTileCache + "\\dem\\Merged4\\{0}\\{1}\\", level, tileX, tileY);



            if (!File.Exists(filename))
            {
                context.Response.ContentType = "image/png";
                Stream s = PlateFile2.GetFileStream(String.Format(wwtTilesDir + "\\{0}.plate", file), -1, level, tileX, tileY);

                int length = (int)s.Length;
                if (length == 0)
                {

                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("No image");
                    context.Response.End();
                    return;
                }
                byte[] data = new byte[length];
                s.Read(data, 0, length);
                context.Response.OutputStream.Write(data, 0, length);
                context.Response.Flush();
                context.Response.End();
                return;

            }

            context.Response.WriteFile(filename);
        }
    }
}
