using System;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class DemMarsProvider : DemMars
    {
        public override void Run(WwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level < 18)
            {
                Stream s = PlateFile2.GetFileStream(@"\\wwtfiles.file.core.windows.net\wwtmars\MarsDem\marsToastDem.plate", -1, level, tileX, tileY);

                if (s == null || (int)s.Length == 0)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("No image");
                    context.Response.End();
                    return;
                }

                int length = (int)s.Length;
                byte[] data = new byte[length];
                s.Read(data, 0, length);
                context.Response.OutputStream.Write(data, 0, length);
                context.Response.Flush();
                context.Response.End();
                return;
            }
        }
    }
}
