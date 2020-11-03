using WWT.Imaging;
using WWTWebservices;

namespace WWT.Providers
{
    public class WMSMoonProvider : WmsBase
    {
        public WMSMoonProvider(IToastTileMapBuilder toastTileMap)
            : base(toastTileMap)
        {
        }

        protected override ImageSource Source => ImageSource.OnMoon;
    }
}
