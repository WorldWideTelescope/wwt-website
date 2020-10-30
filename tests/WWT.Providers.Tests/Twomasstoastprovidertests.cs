using System.IO;
using WWTWebservices;

namespace WWT.Providers.Tests
{
    public class TwoMassToastProviderTests : ProviderTests<TwoMassToastProvider>
    {
        protected override int MaxLevel => 7;

        protected override Stream GetStreamFromPlateTilePyramid(IPlateTilePyramid plateTiles, int level, int x, int y)
            => plateTiles.GetStream(Options.WwtTilesDir, "2MassToast0to7.plate", level, x, y);
    }
}
