using System;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class HiriseDem2Provider : HiriseDem2
    {
        public HiriseDem2Provider(IPlateTilePyramid plateTiles, FilePathOptions options)
            : base(plateTiles, options)
        {
        }

        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            string filename = $@"\\wwt-mars\marsroot\dem\Merged4\{level}\{tileX}\DL{level}X{tileX}Y{tileY}.dem";

            if (File.Exists(filename))
            {
                using (Stream stream = File.OpenRead(filename))
                using (Stream s = MergeMolaDemTileStream(level, tileX, tileY, stream))
                {
                    if (s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                        return;
                    }

                    s.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }
            else
            {
                using (Stream ss = GetMolaDemTileStream(level, tileX, tileY))
                {
                    if (ss.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                        return;
                    }

                    ss.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }
        }
    }
}
