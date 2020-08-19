using System;
using System.IO;
using System.Drawing;
using System.Security.Cryptography;
using WWTWebservices;

namespace WWTMVC5.WWTWeb
{




    public partial class HiRise : System.Web.UI.Page
    {
        private static MD5 md5Hash = MD5.Create();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public Bitmap DownloadBitmap(string dataset, int level, int x, int y)
        {
            //todo fix this
            string DSSTileCache = ""; //; Util.GetCurrentConfigShare("DSSTileCache", true);
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


            string filename = String.Format(DSSTileCache + "\\wwtcache\\mars\\{3}\\{0}\\{2}\\{1}_{2}.png", level, x, y,
                id);
            string path = String.Format(DSSTileCache + "\\wwtcache\\mars\\{3}\\{0}\\{2}", level, x, y, id);


            if (!File.Exists(filename))
            {
                return null;
            }

            return new Bitmap(filename);
        }

        public UInt32 ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }


    }
}