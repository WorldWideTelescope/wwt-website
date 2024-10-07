#nullable enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;

namespace WWT.Providers;

public class MarsHiriseProvider(
    IPlateTilePyramid plateTiles,
    WwtOptions options,
    ILogger<MarsHiriseProvider> logger,
    [FromKeyedServices(Constants.ActivitySourceName)] ActivitySource activitySource)
{
    private const int Width = 256;
    private const int Height = 256;

    public async Task<Image?> GetImageAsync(int level, int tileX, int tileY, int id, CancellationToken token)
    {
        if (level > 17)
        {
            return null;
        }

        using var activity = activitySource.StartImageProcessing();

        var image = await GetBaseImage(level, tileX, tileY, token);

        // Apply HiRise overlay if available
        using var hiRiseOverlay = await LoadHiRiseAsync(level, tileX, tileY, id, token);

        if (hiRiseOverlay is { })
        {
            const float Opacity = 0.5f; // This is the opacity of the overlay that will mimic System.Drawing default CompositingMode=SourceOver

            image.Mutate(c =>
            {
                c.DrawImage(hiRiseOverlay, Opacity);
            });
        }

        return image;
    }

    private async Task<Image> GetBaseImage(int level, int tileX, int tileY, CancellationToken token)
    {
        if (level > 8)
        {
            int levelDif = level - 8;
            int scale = (int)Math.Pow(2, levelDif);
            int tx = tileX / scale;
            int ty = tileY / scale;

            int offsetX = (tileX - (tx * scale)) * (Width / scale);
            int offsetY = (tileY - (ty * scale)) * (Width / scale);
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

            using var stream = await plateTiles.GetStreamAsync(options.WwtTilesDir, "marsbasemap.plate", -1, 8, tx, ty, token);
            var image = await Image.LoadAsync(stream, token);

            image.Mutate(c =>
            {
                c.Crop(new Rectangle(offsetX, offsetY, (int)width, (int)height));
                c.Resize(Width, Height);
            });

            return image;
        }
        else
        {
            using var stream = await plateTiles.GetStreamAsync(options.WwtTilesDir, "marsbasemap.plate", -1, level, tileX, tileY, token);
            var image = await Image.LoadAsync(stream, token);

            image.Mutate(c =>
            {
                c.Crop(Width, Height);
            });

            return image;
        }
    }

    private async Task<Image?> LoadHiRiseAsync(int level, int tileX, int tileY, int id, CancellationToken token)
    {
        var index = ComputeHash(level, tileX, tileY) % 300;

        try
        {
            using var stream = await plateTiles.GetStreamAsync("https://marsstage.blob.core.windows.net/hirise", $"hiriseV5_{index}.plate", id, level, tileX, tileY, token);

            if (stream is null)
            {
                return null;
            }

            var hiRiseOverlay = await Image.LoadAsync(stream, token);

            hiRiseOverlay.Mutate(c =>
            {
                c.Crop(Width, Height);
            });

            return hiRiseOverlay;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to find hirise overlay");
            return null;
        }

        static uint ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }
    }
}
