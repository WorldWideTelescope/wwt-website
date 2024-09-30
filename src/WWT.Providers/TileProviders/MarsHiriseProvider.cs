#nullable disable

using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/MarsHirise.aspx")]
    public class MarsHiriseProvider : RequestProvider
    {
        private readonly ActivitySource _activitySource;
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public MarsHiriseProvider(IPlateTilePyramid plateTiles, WwtOptions options, [FromKeyedServices("WTT")]ActivitySource activitySource)
        {
            _activitySource = activitySource;
            _plateTiles = plateTiles;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            const int Width = 256;
            const int Height = 256;

            (var errored, var level, var tileX, var tileY, var idText) = await HandleLXYExtraQParameter(context, token);
            if (errored)
                return;

            var id = -1;

            if (idText.Length > 0)
            {
                try
                {
                    id = Convert.ToInt32(idText);
                }
                catch
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = ContentTypes.Text;
                    await context.Response.WriteAsync("HTTP/400 illegal HiRISE \"id\" parameter", token);
                    return;
                }
            }

            if (level > 17)
            {
                context.Response.StatusCode = 404;
                return;
            }

            using var activity = _activitySource.StartImageProcessing();

            int ll = level;
            int xx = tileX;
            int yy = tileY;

            if (ll > 8)
            {
                int levelDif = ll - 8;
                int scale = (int)Math.Pow(2, levelDif);
                int tx = xx / scale;
                int ty = yy / scale;

                int offsetX = (xx - (tx * scale)) * (Width / scale);
                int offsetY = (yy - (ty * scale)) * (Width / scale);
                float width = Math.Max(2, (Width / scale));
                float height = width;
                if ((width + offsetX) >= (Width - 1))
                {
                    width -= 1;
                }
                if ((height + offsetY) >= (Height - 1))
                {
                    height -= 1;
                }

                using var stream = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "marsbasemap.plate", -1, 8, tx, ty, token);
                using var bmpl = Image.Load(stream);

                bmpl.Mutate(c =>
                {
                    c.Crop(new Rectangle(offsetX, offsetY, (int)width, (int)height));
                    c.Resize(Width, Height);
                });

                await bmpl.RespondPngAsync(context.Response, token);
            }
            else
            {
                using var stream = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "marsbasemap.plate", -1, ll, xx, yy, token);
                using var bmp1 = Image.Load(stream);

                bmp1.Mutate(c =>
                {
                    c.Crop(Width, Height);
                });

                await bmp1.RespondPngAsync(context.Response, token);
            }

            try
            {
                using (var stream = await LoadHiRiseAsync(ll, xx, yy, id, token))
                {
                    if (stream != null)
                    {
                        using var bmp2 = await Image.LoadAsync(stream, token);

                        bmp2.Mutate(c =>
                        {
                            c.Crop(Width, Height);
                        });
                        await bmp2.RespondPngAsync(context.Response, token);
                    }
                }
            }
            catch
            {
                using var bmp2 = new Image<Rgba32>(Width, Height);
                await bmp2.RespondPngAsync(context.Response, token);
            }
        }

        private Task<Stream> LoadHiRiseAsync(int level, int tileX, int tileY, int id, CancellationToken token)
        {
            UInt32 index = ComputeHash(level, tileX, tileY) % 300;

            return _plateTiles.GetStreamAsync("https://marsstage.blob.core.windows.net/hirise", $"hiriseV5_{index}.plate", id, level, tileX, tileY, token);
        }

        private UInt32 ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }
    }
}
