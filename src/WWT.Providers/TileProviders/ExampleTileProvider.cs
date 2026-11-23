#nullable disable

using Microsoft.Extensions.DependencyInjection;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/Example.aspx")]
    public class ExampleTileProvider : RequestProvider
    {
        private readonly ActivitySource _activitySource;

        public ExampleTileProvider([FromKeyedServices(Constants.ActivitySourceName)] ActivitySource activitySource)
        {
            _activitySource = activitySource;
        }

        public override string ContentType => ContentTypes.OctetStream;


        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            var size = 256;
            using var image = new Image<Rgba32>(size, size);
            var random = Random.Shared;
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);

            var lineColor = Color.FromPixel(new Rgba32(177, 194, 222, 255));
            image.Mutate(ctx =>
            {
                ctx.Fill(Color.FromPixel(new Rgba32(r, g, b, 255)));

                // Draw a grid every 16 rows and columns
                for (int i = 0; i < 256; i += 32)
                {
                    ctx.DrawLine(lineColor, thickness: 1f, new PointF(i, 0), new PointF(i, 256));
                    ctx.DrawLine(lineColor, thickness: 1f, new PointF(0, i), new PointF(256, i));
                }

                var label = $"L={level} X={tileX} Y={tileY}";
                var fontFamily = SystemFonts.Families.First();
                var font = fontFamily.CreateFont(12, FontStyle.Regular);

                RichTextOptions options = new (font)
                {
                    Origin = new PointF(size / 2f, 50),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    WrappingLength = size - 100,
                };
                ctx.DrawText(options, label, Color.White);
            });

            using var ms = new MemoryStream();
            await image.SaveAsPngAsync(ms, token);

            context.Response.ContentType = "image/png";

            if (ms.Length == 0)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("No image", token);
                context.Response.End();
                return;
            }

            await ms.CopyToAsync(context.Response.OutputStream, token);
            context.Response.Flush();
            context.Response.End();
        }
    }
}