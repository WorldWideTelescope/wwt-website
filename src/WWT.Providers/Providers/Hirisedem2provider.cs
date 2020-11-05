using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class HiriseDem2Provider : HiriseDem2
    {
        public HiriseDem2Provider(IPlateTilePyramid plateTiles, WwtOptions options)
            : base(plateTiles, options)
        {
        }

        public override string ContentType => ContentTypes.Text;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
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
                using (Stream s = await MergeMolaDemTileStream(level, tileX, tileY, stream, token))
                {
                    if (s.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                        return;
                    }

                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }
            else
            {
                using (Stream ss = await GetMolaDemTileStreamAsync(level, tileX, tileY, token))
                {
                    if (ss.Length == 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("No image");
                        context.Response.End();
                        return;
                    }

                    await ss.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }
        }
    }
}
