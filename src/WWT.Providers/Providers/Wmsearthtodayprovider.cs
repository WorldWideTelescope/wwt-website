using WWT.Imaging;
using WWTWebservices;

namespace WWT.Providers
{
    public class WMSEarthTodayProvider : WmsBase
    {
        public WMSEarthTodayProvider(IToastTileMapBuilder toastTileMap)
            : base(toastTileMap)
        {
        }

        protected override ImageSource Source => ImageSource.WmsJpl;
    }
}
