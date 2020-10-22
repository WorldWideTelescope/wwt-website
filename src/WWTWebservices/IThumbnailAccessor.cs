using System.IO;

namespace WWT
{
    public interface IThumbnailAccessor
    {
        Stream GetThumbnailStream(string name, string type);
    }
}
