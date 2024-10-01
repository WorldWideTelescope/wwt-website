
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using WWT.Imaging;

#nullable enable

namespace WWT.Providers
{
    public class OctTileMapBuilder([FromKeyedServices(Constants.ActivitySourceName)] ActivitySource activitySource) : IOctTileMapBuilder
    {
        public Task<Stream?> GetOctTileAsync(int level, int tileX, int tileY, bool enforceBoundary, CancellationToken token)
        {
            var map = new OctTileMap(level, tileX, tileY);

            // SDSS boundaries
            // RA: 105 deg <-> 270 deg
            // DEC: -3 deg <-> + 75 deg
            if (enforceBoundary)
            {
                if (map.raMin > 270 | map.decMax < -3 | map.raMax < 105 | map.decMin > 75)
                {
                    Activity.Current?.SetBaggage("Boundary Enforced", "true");
                    return Task.FromResult<Stream?>(null);
                }
            }

            Int32 sqSide = 256;

            using var activity = activitySource.StartImageProcessing();

            using var bmpOutput = new Image<Rgb24>(sqSide, sqSide);
            var sdim = new SdssImage<Rgb24>(map.raMin, map.decMax, map.raMax, map.decMin, true);
            sdim.LoadImage();

            if (sdim.image != null)
            {
                // Fill up bmp from sdim
                Vector2d vxy, vradec;
                unsafe
                {
                    for (int y = 0; y < sqSide; y++)
                    {
                        vxy.Y = (y / 255.0);
                        for (int x = 0; x < sqSide; x++)
                        {
                            vxy.X = (x / 255.0);
                            vradec = map.PointToRaDec(vxy);
                            bmpOutput[x, y] = sdim.GetPixelDataAtRaDec(vradec);
                        }
                    }
                }

                sdim.image.Dispose();
            }

            var result = bmpOutput.ToPngStream();

            return Task.FromResult<Stream?>(result);
        }
    }
}
