using System;
using System.Configuration;
using WWT.Imaging;
using WWTWebservices;

namespace WWT.Providers
{
    public class WMSToastProvider : WmsBase
    {
        public WMSToastProvider(IToastTileMapBuilder toastTileMap)
            : base(toastTileMap)
        {
        }

        protected override ImageSource Source => ImageSource.MarsAsu;
    }
}
