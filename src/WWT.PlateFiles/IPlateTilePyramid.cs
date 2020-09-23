using System.IO;

namespace WWTWebservices
{
    public interface IPlateTilePyramid
    {
        Stream GetStream(string pathPrefix, string plateName, int level, int x, int y);
    }
}
