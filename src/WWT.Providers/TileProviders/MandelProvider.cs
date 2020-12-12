#nullable disable

using System.Threading;
using System.Threading.Tasks;

using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/Mandel.aspx")]
    public class MandelProvider : RequestProvider
    {
        private readonly IMandelbrot _mandelbrot;

        public MandelProvider(IMandelbrot mandelbrot)
        {
            _mandelbrot = mandelbrot;
        }

        public override string ContentType => ContentTypes.Jpeg;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            using (var image = _mandelbrot.CreateMandelbrot(level, tileX, tileY))
            {
                await image.CopyToAsync(context.Response.OutputStream, token);
            }
        }
    }
}
