#nullable disable

namespace WWT.Providers
{
    public class WwtOptions
    {
        public int TourVersionCheckIntervalMinutes { get; set; }

        public bool LoginTracking { get; set; }

        public string LoggingConn { get; set; } = string.Empty;

        public string DataDir { get; set; } = string.Empty;

        public string DssTerapixelDir { get; set; } = string.Empty;

        public string DSSTileCache { get; set; } = string.Empty;

        public string DssToastPng { get; set; } = string.Empty;

        public string WWTDEMDir { get; set; } = string.Empty;

        public string WwtTilesDir { get; set; } = string.Empty;

        public string WwtTourCache { get; set; } = string.Empty;

        public string WwtToursTourFileUNC { get; set; } = string.Empty;

        public string WwtGalexDir { get; set; } = string.Empty;

        public string WwtToursDBConnectionString { get; set; } = string.Empty;

        public string ExternalUrlMapText { get; set; } = string.Empty;
    }
}
