using System;
using System.IO;
using WWTWebservices;

namespace WWT.Providers.Tests
{
    public class HiriseDem3ProviderTests : ProviderTests<HiriseDem3Provider>
    {
        protected override Action<IResponse> NullStreamResponseHandler => null;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override Stream GetStreamFromPlateTilePyramid(IPlateTilePyramid plateTiles, int level, int x, int y)
            => plateTiles.GetStream(Options.WwtTilesDir, "marsToastDem.plate", -1, level, x, y);
    }
}
