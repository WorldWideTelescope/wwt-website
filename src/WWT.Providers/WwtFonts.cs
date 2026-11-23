using SixLabors.Fonts;

namespace WWT.Providers
{

    public interface IWwtFonts
    {
        FontFamily Family { get; }
    }
    
    public sealed class WwtFonts : IWwtFonts
    {
        public FontFamily Family { get; }
    
        public WwtFonts()
        {
            var collection = new FontCollection();
            using var stream = typeof(WwtFonts).Assembly
                .GetManifestResourceStream("WWT.Providers.Fonts.micross.ttf")
                ?? throw new System.InvalidOperationException("Embedded font missing");
            Family = collection.Add(stream);
        }
    }
}
