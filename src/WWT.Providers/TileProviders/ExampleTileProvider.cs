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
        private readonly IWwtFonts _fonts;

        public ExampleTileProvider(IWwtFonts fonts)
        {
            _fonts = fonts;
        }

        public override string ContentType => ContentTypes.Png;


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

                // Draw a grid every 32 rows and columns
                for (int i = 0; i < 256; i += 32)
                {
                    ctx.DrawLine(lineColor, thickness: 1f, new PointF(i, 0), new PointF(i, 256));
                    ctx.DrawLine(lineColor, thickness: 1f, new PointF(0, i), new PointF(256, i));
                }

                var label = $"L={level} X={tileX} Y={tileY}";
                var font = _fonts.Family.CreateFont(25, FontStyle.Regular);

                RichTextOptions options = new (font)
                {
                    Origin = new PointF(size / 2f, size / 2f),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    WrappingLength = size - 100,
                };
                ctx.DrawText(options, label, Color.White);
            });

            await image.RespondPngAsync(context.Response, token); 
        }
    }
}
