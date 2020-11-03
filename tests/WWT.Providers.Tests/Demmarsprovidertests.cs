using System;
using System.IO;
using System.Threading.Tasks;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public class DemMarsProviderTests : ProviderTests<DemMarsProvider>
    {
        protected override int MaxLevel => 17;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            Assert.Empty(response.OutputStream.ToArray());
        }

        protected override Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            return plateTiles.GetStreamAsync(@"\\wwtfiles.file.core.windows.net\wwtmars\MarsDem", "marsToastDem.plate", -1, level, x, y, default);
        }
    }
}
