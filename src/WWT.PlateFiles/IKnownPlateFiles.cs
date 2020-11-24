#nullable disable

namespace WWT.PlateFiles
{
    public interface IKnownPlateFiles
    {
        bool TryNormalizePlateName(string input, out string platefile);
    }
}
