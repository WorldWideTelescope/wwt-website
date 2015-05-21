using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

namespace WWTWebservices
{



    public class WWTUtil
    {
        public WWTUtil()
        {
        }

        public static string GetCurrentConfigShare(string entryName, bool checkAlive)
        {
            string primary = ConfigurationManager.AppSettings["PrimaryFileserver"].ToLower();
            string backup = ConfigurationManager.AppSettings["BackupFileserver"].ToLower();

            string current = (string) HttpContext.Current.Cache.Get("CurrentFileServer");

            if (checkAlive || string.IsNullOrEmpty(current))
            {
                DateTime lastCheck = DateTime.Now.AddDays(-1);

                if (!string.IsNullOrEmpty(current) &&
                    HttpContext.Current.Cache.Get("LastFileserverUpdateDateTime") != null)
                {
                    lastCheck = (DateTime) HttpContext.Current.Cache.Get("LastFileserverUpdateDateTime");
                }

                TimeSpan ts = DateTime.Now - lastCheck;

                if (ts.TotalMinutes > 1)
                {
                    HttpContext.Current.Cache.Remove("LastFileserverUpdateDateTime");
                    HttpContext.Current.Cache.Add("LastFileserverUpdateDateTime", System.DateTime.Now, null,
                        DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);


                    if (string.IsNullOrEmpty(current) || !Directory.Exists(@"\\" + current + @"\DSSTileCache\dsstoast"))
                    {
                        bool primaryUp = false;

                        try
                        {
                            primaryUp = Directory.Exists(@"\\" + primary + @"\DSSTileCache\dsstoast");

                        }
                        catch
                        {
                        }

                        if (primaryUp)
                        {
                            current = primary;
                            HttpContext.Current.Cache.Remove("CurrentFileServer");
                            HttpContext.Current.Cache.Add("CurrentFileServer", current, null, DateTime.MaxValue,
                                new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

                        }
                        else
                        {
                            current = backup;
                            HttpContext.Current.Cache.Remove("CurrentFileServer");
                            HttpContext.Current.Cache.Add("CurrentFileServer", current, null, DateTime.MaxValue,
                                new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                        }
                    }
                }
            }



            string baseName = ConfigurationManager.AppSettings[entryName].ToLower();

            return baseName.Replace(primary, current);

        }

        public static bool ShouldDownloadSDSS(int level, int xtile, int ytile)
        {
            // SDSS boundaries
            // RA: 105 deg <-> 270 deg
            // DEC: -3 deg <-> + 75 deg

            int N_y = (int) Math.Pow(2, level);
            int N_x = 2*N_y;

            ytile = N_y - ytile;

            int SDSSxTileMin = (int) Math.Floor(105.0/360*N_x);
            int SDSSxTileMax = (int) Math.Floor(270.0/360*N_x);
            int SDSSyTileMin = (int) Math.Floor(15.0/180*N_y);
            int SDSSyTileMax = (int) Math.Floor(93.0/180*N_y);

            if (xtile >= SDSSxTileMin && xtile <= SDSSxTileMax && ytile >= SDSSyTileMin && ytile <= SDSSyTileMax)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static int GetServerID(int x, int y)
        {
            int server = (x & 1) + ((y & 1) << 1);

            return (server);
        }

        private static string[] googleTileMap = new string[] {"q", "r", "t", "s"};

        public static string GetTileID(int x, int y, int level, bool GoogleStyle)
        {
            StringBuilder sb = new StringBuilder();
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

        public static int GetTileAddressFromVEKey(string veKey, out int x, out int y)
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

        public static string DownloadVeTile(int level, int tileX, int tileY, bool temp)
        {
            string DSSTileCache = GetCurrentConfigShare("DSSTileCache", true);
            string filename = String.Format(DSSTileCache + "\\VE\\level{0}\\{2}\\{1}_{2}.jpg", level, tileX, tileY);
            string path = String.Format(DSSTileCache + "\\VE\\level{0}\\{2}", level, tileX, tileY);

            if (temp)
            {
                filename = filename + "tmp.jpg";
            }

            if (!File.Exists(filename))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                System.Net.WebClient client = new System.Net.WebClient();


                double tileWidth = (180/(Math.Pow(2, level)));
                double top = ((double) tileY*tileWidth) - 90.0;
                double bot = top + tileWidth;
                double left = ((double) tileX*tileWidth) - 180.0;
                double right = left + tileWidth;

                string url = String.Format("http://a{0}.ortho.tiles.virtualearth.net/tiles/a{1}.jpeg?g=15",
                    GetServerID(tileX, tileY), GetTileID(tileX, tileY, level, false));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                client.DownloadFile(url, filename);

                client.Dispose();

                return filename;

            }
            return filename;
        }

    }
}
