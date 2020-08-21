using System;
using System.IO;

namespace WWTWebservices
{
    public static class DemCodec
    {
        public static DemTile Decompress(string filename) => throw new NotImplementedException();

        public static DemTile Decompress(Stream stream) => throw new NotImplementedException();
    }

    public class DemTile
    {
        public double AltitudeInMeters(int row, int col) => throw new NotImplementedException();
    }
}
