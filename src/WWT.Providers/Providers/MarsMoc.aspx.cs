using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public abstract partial class MarsMoc : RequestProvider
    {
        public Bitmap DownloadBitmap(string dataset, int level, int x, int y)
        {
            string DSSTileCache = ConfigurationManager.AppSettings["DSSTileCache"];
            string id = "1738422189";
            switch (dataset)
            {
                case "mars_base_map":
                    id = "1738422189";
                    break;
                case "mars_terrain_color":
                    id = "220581050";
                    break;
                case "mars_hirise":
                    id = "109459728";
                    break;
                case "mars_moc":
                    id = "252927426";
                    break;
                case "mars_historic_green":
                    id = "1194136815";
                    break;
                case "mars_historic_schiaparelli":
                    id = "1113282550";
                    break;
                case "mars_historic_lowell":
                    id = "675790761";
                    break;
                case "mars_historic_antoniadi":
                    id = "1648157275";
                    break;
                case "mars_historic_mec1":
                    id = "2141096698";
                    break;

            }


            string filename = String.Format(DSSTileCache + "\\wwtcache\\mars\\{3}\\{0}\\{2}\\{1}_{2}.png", level, x, y, id);
            string path = String.Format(DSSTileCache + "\\wwtcache\\mars\\{3}\\{0}\\{2}", level, x, y, id);


            if (!File.Exists(filename))
            {
                return null;
            }

            return new Bitmap(filename);
        }

        public Bitmap LoadMoc(int level, int tileX, int tileY)
        {
            UInt32 index = ComputeHash(level, tileX, tileY) % 400;

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(String.Format(@"https://marsstage.blob.core.windows.net/moc/mocv5_{0}.plate", index)));

            Stream stream = blob.OpenRead();

            Stream s = PlateFile2.GetFileStream(stream, -1, level, tileX, tileY);

            if (s != null)
            {
                return new Bitmap(s);
            }
            return null;

        }

        public UInt32 ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }
    }
}
