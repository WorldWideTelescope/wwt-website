#nullable disable

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers.Services
{
    public class VirtualEarthDownloader : IVirtualEarthDownloader
    {
        private static readonly string[] googleTileMap = new string[] { "q", "r", "t", "s" };

        private readonly HttpClient _httpClient;

        public VirtualEarthDownloader()
        {
            _httpClient = new HttpClient();
        }

        public int GetServerID(int x, int y)
        {
            int server = (x & 1) + ((y & 1) << 1);

            return (server);
        }

        public string GetTileID(int x, int y, int level, bool GoogleStyle)
        {
            var sb = new StringBuilder();

            if (GoogleStyle)
            {
                sb.Append("t");
            }

            for (int i = level; i > 0; --i)
            {
                int mask = 1 << (i - 1);
                int val = 0;

                if ((x & mask) != 0)
                    val = 1;

                if ((y & mask) != 0)
                    val += 2;
                if (GoogleStyle)
                {
                    sb.Append(googleTileMap[val]);
                }
                else
                {
                    sb.Append(val);
                }
            }
            return (sb.ToString());
        }

        public int GetTileAddressFromVEKey(string veKey, out int x, out int y)
        {
            int tileX = 0;
            int tileY = 0;
            int addValue = 1;
            int level = 0;
            while (veKey.Length > 0)
            {
                int val = Convert.ToInt32(veKey.Substring(veKey.Length - 1, 1));

                switch (val)
                {
                    case 0:
                        break;
                    case 1:
                        tileX += addValue;
                        break;
                    case 2:
                        tileY += addValue;
                        break;
                    case 3:
                        tileX += addValue;
                        tileY += addValue;
                        break;

                }
                addValue *= 2;
                level++;
                if (veKey.Length > 1)
                {
                    veKey = veKey.Substring(0, veKey.Length - 1);
                }
                else
                {
                    veKey = "";
                }
            }
            x = tileX;
            y = tileY;
            return level;
        }

        public Task<Stream> DownloadVeTileAsync(VirtualEarthTile tileType, int level, int tileX, int tileY, CancellationToken token)
        {
            var id = GetTileID(tileX, tileY, level, false);
            var server = GetServerID(tileX, tileY);
            var url = tileType switch
            {
                VirtualEarthTile.Ortho => $"http://a{server}.ortho.tiles.virtualearth.net/tiles/a{id}.jpeg?g=15",
                VirtualEarthTile.Ecn => $"http://ecn.t{server}.tiles.virtualearth.net/tiles/d{id}.elv?g=1&n=z",
                _ => throw new NotImplementedException()
            };

            return _httpClient.GetStreamAsync(url);
        }
    }
}
