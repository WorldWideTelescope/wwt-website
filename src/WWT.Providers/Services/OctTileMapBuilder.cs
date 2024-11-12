
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using WWT.Imaging;

#nullable enable

namespace WWT.Providers;

public class OctTileMapBuilder([FromKeyedServices(Constants.ActivitySourceName)] ActivitySource activitySource, IHttpClientFactory httpClientFactory) : IOctTileMapBuilder
{
    public async Task<Stream?> GetOctTileAsync(int level, int tileX, int tileY, bool enforceBoundary, CancellationToken token)
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
                return null;
            }
        }

        using var activity = activitySource.StartImageProcessing();

        const int sqSide = 256;
        using var bmpOutput = new Image<Rgb24>(sqSide, sqSide);

        using var loaded = await LoadImage(map.raMin, map.decMax, map.raMax, map.decMin, true, token);

        if (loaded is { } sdss)
        {
            for (int y = 0; y < sqSide; y++)
            {
                var vxy = new Vector2d(0, (y / 255.0));

                for (int x = 0; x < sqSide; x++)
                {
                    vxy.X = (x / 255.0);
                    var vradec = map.PointToRaDec(vxy);
                    bmpOutput[x, y] = sdss.GetPixelDataAtRaDec(vradec);
                }
            }
        }

        return bmpOutput.ToPngStream();
    }

    private async Task<SdssImage?> LoadImage(double raLeft, double decTop, double raRight, double decBottom, bool dr12, CancellationToken token)
    {
        var innerPath = dr12 ? "dr12" : "dr6";
        var raCenter = (raLeft + raRight) / 2.0;
        var decCenter = (decBottom + decTop) / 2.0;
        var scale = Math.Abs(decBottom - decTop) / 500.0;
        var updatedScale = scale * 3600.0;
        var address = dr12 ?
        $"https://skyserver.sdss.org/{innerPath}/SkyServerWS/ImgCutout/getjpeg?TaskName=SkyServer.Chart.List&ra={raCenter}&dec={decCenter}&scale={updatedScale}&width={512.0}&height={512.0}&opt="
        :
        $"https://skyservice.pha.jhu.edu/{innerPath}/imgcutout/getjpeg.aspx?ra={raCenter}&dec={decCenter}&scale={updatedScale}&width={512.0}&height={512.0}&opt=&query=";

        using var client = httpClientFactory.CreateClient();
        using var response = await client.GetAsync(address, token);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        using var stream = response.Content.ReadAsStream(token);

        if (stream.Length > 8000)
        {
            return new(await Image.LoadAsync<Rgb24>(stream, token), decCenter, raCenter, scale);
        }
        else
        {
            return null;
        }
    }

    private readonly struct SdssImage(Image<Rgb24> image, double decCenter, double raCenter, double scale) : IDisposable
    {
        public Image<Rgb24> Image => image;

        public Rgb24 GetPixelDataAtRaDec(Vector2d raDec)
        {
            const double xoff = 256.0;
            const double yoff = 256.0;

            return Image[(int)(xoff - (raDec.X - raCenter) * Math.Cos(raDec.Y * (Math.PI / 180.0)) / scale), (int)(yoff - (raDec.Y - decCenter) / scale)];
        }

        public void Dispose()
        {
            Image.Dispose();
        }
    }
}
