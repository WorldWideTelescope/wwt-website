using System;
using System.Configuration;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class HiriseDem2Provider : HiriseDem2
    {
        public override void Run(WwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
            string DSSTileCache = WWTUtil.GetCurrentConfigShare("DSSTileCache", true);

            DSSTileCache = @"\\wwt-mars\marsroot";

            string filename = String.Format(DSSTileCache + "\\dem\\Merged4\\{0}\\{1}\\DL{0}X{1}Y{2}.dem", level, tileX, tileY);

            string path = String.Format(DSSTileCache + "\\dem\\Merged4\\{0}\\{1}\\", level, tileX, tileY);



            if (File.Exists(filename))
            {
                Stream stream = File.OpenRead(filename);
                Stream s = MergeMolaDemTileStream(level, tileX, tileY, stream);

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

            {



                Stream ss = GetMolaDemTileStream(level, tileX, tileY);

                int len = (int)ss.Length;
                if (len == 0)
                {

                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("No image");
                    context.Response.End();
                    return;
                }
                byte[] data = new byte[len];
                ss.Read(data, 0, len);
                context.Response.OutputStream.Write(data, 0, len);
                context.Response.Flush();
                context.Response.End();
                return;
            }
        }
    }
}
