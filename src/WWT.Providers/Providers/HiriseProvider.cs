using System;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class HiriseProvider : HiRise
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level < 19)
            {
                context.Response.ContentType = "image/png";

                UInt32 index = ComputeHash(level, tileX, tileY) % 300;


                Stream s = PlateFile2.GetFileStream(String.Format(@"\\wwt-mars\marsroot\hirise\hiriseV5_{0}.plate", index), -1, level, tileX, tileY);

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
