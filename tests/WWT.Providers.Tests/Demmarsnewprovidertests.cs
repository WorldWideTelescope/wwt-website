using System;
using System.IO;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public class DemMarsNewProviderTests : ProviderTests<DemMarsNewProvider>
    {
        protected override int MaxLevel => 17;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            Assert.Empty(response.OutputStream.ToArray());
        }

        protected override Stream GetStreamFromPlateTilePyramid(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            var index = DirectoryEntry.ComputeHash(level + 128, x, y) % 400;

            return plateTiles.GetStream(@"\\wwt-mars\marsroot\dem\", $"marsToastDem_{index}.plate", -1, level, x, y);
        }
    }
}
