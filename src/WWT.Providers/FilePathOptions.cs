using System.Configuration;
using WWTWebservices;

namespace WWT.Providers
{
    public class FilePathOptions
    {
        public static FilePathOptions CreateFromConfig()
            => new FilePathOptions
            {
                DssTerapixelDir = ConfigurationManager.AppSettings["DssTerapixelDir"],
                DSSTileCache = ConfigurationManager.AppSettings["DSSTileCache"],
                DssToastPng = ConfigurationManager.AppSettings["DSSTOASTPNG"],
                WWTDEMDir = ConfigurationManager.AppSettings["WWTDEMDir"],
                WwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"],
            };

        public string DssTerapixelDir { get; set; }

        public string DSSTileCache { get; set; }

        public string DssToastPng { get; set; }

        public string WWTDEMDir { get; set; }

        public string WwtTilesDir { get; set; }
    }
}
