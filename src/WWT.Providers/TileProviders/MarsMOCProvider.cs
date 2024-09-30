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
    [RequestEndpoint("/wwtweb/MarsMoc.aspx")]
    public class MarsMocProvider : RequestProvider
    {
        private readonly ActivitySource _activitySource;
        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public MarsMocProvider(IPlateTilePyramid plateTiles, WwtOptions options, [FromKeyedServices("WTT")]ActivitySource activitySource)
        {
            _activitySource = activitySource;
            _plateTiles = plateTiles;
            _options = options;
        }

        public override string ContentType => ContentTypes.Png;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            (var errored, var level, var tileX, var tileY) = await HandleLXYQParameter(context, token);
            if (errored)
                return;

            using var activity = _activitySource.StartImageProcessing();

            using (var output = new Image<Rgba32>(256, 256))
            {
                // TODO: This level was set to 15 before. Should identify a
                // better way to know if a level is beyond the dataset
                // without hardcoding.
                if (level > 14)
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                int ll = level;
                int xx = tileX;
                int yy = tileY;

                if (ll > 8)
                {
                    int levelDif = ll - 8;
                    int scale = (int) Math.Pow(2, levelDif);
                    int tx = xx / scale;
                    int ty = yy / scale;

                    int offsetX = (xx - (tx * scale)) * (256 / scale);
                    int offsetY = (yy - (ty * scale)) * (256 / scale);
                    float width = 256 / scale;
                    float height = width;

                    if ((width + offsetX) >= 255)
                    {
                        width -= 1;
                    }
                    if ((height + offsetY) >= 255)
                    {
                        height -= 1;
                    }

                    using (var stream = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "marsbasemap.plate", -1, 8, tx, ty, token))
                    using (var bmp1 = Image.Load(stream))
                    {
                        bmp1.Mutate(x => x.Crop(new Rectangle(offsetX, offsetY, (int) width, (int) height)).Resize(256, 256));
                        output.Mutate(x => x.DrawImage(bmp1, 1.0f));
                    }
                }
                else
                {
                    using (var stream = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "marsbasemap.plate", -1, ll, xx, yy, token))
                    using (var bmp1 = Image.Load(stream))
                    {
                        output.Mutate(x => x.DrawImage(bmp1, 1.0f));
                    }
                }

                using (var stream = await LoadMocAsync(ll, xx, yy, token))
                {
                    if (stream != null)
                    {
                        using (var bmp2 = Image.Load(stream))
                        {
                            output.Mutate(x => x.DrawImage(bmp2, 1.0f));
                        }
                    }
                }

                await output.RespondPngAsync(context.Response, token);
            }
        }

        private Task<Stream> LoadMocAsync(int level, int tileX, int tileY, CancellationToken token)
        {
            UInt32 index = ComputeHash(level, tileX, tileY) % 400;

            return _plateTiles.GetStreamAsync("https://marsstage.blob.core.windows.net/moc", $"mocv5_{index}.plate", -1, level, tileX, tileY, token);
        }

        private UInt32 ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }
    }
}
