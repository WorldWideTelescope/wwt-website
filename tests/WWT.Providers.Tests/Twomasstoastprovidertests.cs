using System.IO;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers.Tests
{
    public class TwoMassToastProviderTests : ProviderTests<TwoMassToastProvider>
    {
        protected override int MaxLevel => 7;

        protected override Task<Stream> GetStreamFromPlateTilePyramidAsync(IPlateTilePyramid plateTiles, int level, int x, int y)
            => plateTiles.GetStreamAsync(Options.WwtTilesDir, "2MassToast0to7.plate", level, x, y, default);
    }
}
