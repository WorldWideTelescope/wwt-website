using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

using WWT.PlateFiles;

namespace WWT.Providers.Tests
{
    public class g360ProviderTests : ProviderTests<G360Provider>
    {
        protected override int MaxLevel => 11;

        protected override Action<IResponse> StreamExceptionResponseHandler => null;

        protected override Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y)
        {
            var index = DirectoryEntry.ComputeHash(level + 128, x, y) % 16;

            return plateTiles.GetStreamAsync(Options.WwtTilesDir, $"g360-{index}.plate", -1, level, x, y, default);
        }

        protected override void ExpectedResponseAboveMaxLevel(IResponse response)
        {
            Assert.Empty(response.ContentType);
            Assert.Empty(response.OutputStream.ToArray());
        }

           }
}
