#nullable disable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/HiriseDem2.aspx")]
    public class HiriseDem2Provider : HiriseDem2
    {
        public HiriseDem2Provider(IPlateTilePyramid plateTiles, WwtOptions options)
            : base(plateTiles, options)
        {
        }

        public override string ContentType => ContentTypes.Text;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

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
                        await context.Response.WriteAsync("No image", token);
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
                        await context.Response.WriteAsync("No image", token);
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
