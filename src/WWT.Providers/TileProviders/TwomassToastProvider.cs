#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/TwoMassToast.aspx")]
    public class TwoMassToastProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public TwoMassToastProvider(IPlateTilePyramid plateTiles, WwtOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            if (level > 7)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("No image", token);
                context.Response.End();
                return;;
            }
            else
            {
                try
                {
                    context.Response.ContentType = "image/png";

                    using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "2MassToast0to7.plate", level, tileX, tileY, token))
                    {
                        await s.CopyToAsync(context.Response.OutputStream, token);
                        context.Response.Flush();
                        context.Response.End();
                        return;;
                    }
                }
                catch
                {
                    context.Response.Clear();
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("No image", token);
                    context.Response.End();
                    return;;
                }
            }
        }
    }
}
