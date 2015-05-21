using System;
using System.IO;
using System.Drawing;
using WWTWebservices;

namespace WWTMVC5.WWTWeb
{

    public class MarsHirise : System.Web.UI.Page
    {

        public Bitmap DownloadBitmap(string dataset, int level, int x, int y)
        {
            string DSSTileCache = WWTUtil.GetCurrentConfigShare("DSSTileCache", true);
            string id = "1738422189";
            string type = ".png";
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
                    type = ".auto";
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


            string filename = String.Format(DSSTileCache + "\\wwtcache\\mars\\{3}\\{0}\\{2}\\{1}_{2}.png", level, x, y,
                id);
            string path = String.Format(DSSTileCache + "\\wwtcache\\mars\\{3}\\{0}\\{2}", level, x, y, id);


            if (!File.Exists(filename))
            {
                return null;
            }

            return new Bitmap(filename);
        }

        public Bitmap LoadHiRise(int level, int tileX, int tileY, int id)
        {
            UInt32 index = ComputeHash(level, tileX, tileY)%300;


            Stream s = PlateFile2.GetFileStream(String.Format(@"\\wwt-mars\marsroot\hirise\hiriseV5_{0}.plate", index),
                id, level, tileX, tileY);
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
