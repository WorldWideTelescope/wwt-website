#nullable disable

using System.Drawing;
using System.IO;

namespace WWT.Providers
{
    public interface IMandelbrot
    {
        Stream CreateMandelbrot(int level, int tileX, int tileY);
    }
}
