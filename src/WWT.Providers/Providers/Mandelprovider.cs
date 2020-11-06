using System;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
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
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            using (var image = _mandelbrot.CreateMandelbrot(level, tileX, tileY))
            {
                await image.CopyToAsync(context.Response.OutputStream, token);
            }
        }
    }
}
