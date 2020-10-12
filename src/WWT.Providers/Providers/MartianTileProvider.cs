using System;
using System.IO;

namespace WWT.Providers
{
    public class MartianTileProvider : RequestProvider
    {
        private readonly FilePathOptions _options;

        public MartianTileProvider(FilePathOptions options)
        {
            _options = options;
        }

        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string dataset = values[3];
            string id = "nothing";

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

            string filename = $@"{_options.DSSTileCache}\wwtcache\mars\{id}\{level}\{tileY}\{tileX}_{tileY}.png";

            if (!File.Exists(filename))
            {
                context.Response.StatusCode = 404;
            }
            else
            {
                context.Response.WriteFile(filename);
            }
        }
    }
}
