using System;
using System.IO;
using WWTWebservices;
using Xunit;

namespace WWT.Providers.Tests
{
    public class g360ProviderTests : ProviderTests<g360Provider>
    {
        protected override int MaxLevel => 11;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override Stream GetStreamFromPlateTilePyramid(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            var index = DirectoryEntry.ComputeHash(level + 128, x, y) % 16;

            return plateTiles.GetStream(Options.WwtTilesDir, $"g360-{index}.plate", -1, level, x, y);
        }

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            Assert.Empty(response.ContentType);
            Assert.Empty(response.OutputStream.ToArray());
        }

           }
}
