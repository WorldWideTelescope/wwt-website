namespace WWT.Providers
{
    public class WwtOptions
    {
        public int TourVersionCheckIntervalMinutes { get; set; }

        public bool LoginTracking { get; set; }

        public string LoggingConn { get; set; }

        public string Webkey { get; set; }

        public string DataDir { get; set; }

        public string DssTerapixelDir { get; set; }

        public string DSSTileCache { get; set; }

        public string DssToastPng { get; set; }

        public string WWTDEMDir { get; set; }

        public string WwtTilesDir { get; set; }

        public string WwtTourCache { get; set; }

        public string WwtToursTourFileUNC { get; set; }

        public string WwtGalexDir { get; set; }

        public string WwtToursDBConnectionString { get; set; }
    }
}
