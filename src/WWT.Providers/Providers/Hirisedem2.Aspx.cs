using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public abstract partial class HiriseDem2 : RequestProvider
    {
        private static byte[] backslashXIndex = new byte[]
        {
            0, 8, 16, 0, 8, 16, 0, 8, 16, 4, 4, 8, 4, 0, 4, 6, 6, 8, 6, 8, 6, 4, 2, 2, 6, 6, 4, 2, 0, 2, 4, 6, 6, 2, 2, 0,
            2, 4, 2, 7, 7, 8, 7, 8, 7, 6, 5, 5, 7, 7, 6, 7, 8, 7, 6, 5, 5, 7, 7, 8, 7, 6, 7, 2, 1, 1, 3, 3, 4, 3, 4, 3,
            2, 3, 3, 5, 5, 4, 5, 4, 5, 6, 7, 7, 5, 5, 6, 1, 0, 1, 2, 3, 3, 1, 1, 0, 1, 2, 1, 6, 7, 7, 5, 5, 4, 5, 4, 5,
            6, 5, 5, 1, 1, 0, 1, 0, 1, 2, 3, 3, 1, 1, 2, 3, 4, 3, 2, 1, 1, 3, 3, 4, 3, 2, 3, 12, 12, 16, 12, 8, 12, 14,
            14, 16, 14, 16, 14, 12, 10, 10, 14, 14, 12, 10, 8, 10, 12, 14, 14, 10, 10, 8, 10, 12, 10, 15, 15, 16, 15, 16,
            15, 14, 13, 13, 15, 15, 14, 15, 16, 15, 14, 13, 13, 15, 15, 16, 15, 14, 15, 10, 9, 9, 11, 11, 12, 11, 12, 11,
            10, 11, 11, 13, 13, 12, 13, 12, 13, 14, 15, 15, 13, 13, 14, 9, 8, 9, 10, 11, 11, 9, 9, 8, 9, 10, 9, 14, 15,
            15, 13, 13, 12, 13, 12, 13, 14, 13, 13, 9, 9, 8, 9, 8, 9, 10, 11, 11, 9, 9, 10, 11, 12, 11, 10, 9, 9, 11, 11,
            12, 11, 10, 11, 4, 4, 8, 4, 0, 4, 6, 6, 8, 6, 8, 6, 4, 2, 2, 6, 6, 4, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 7,
            7, 8, 7, 8, 7, 6, 5, 5, 7, 7, 6, 7, 8, 7, 6, 5, 5, 7, 7, 8, 7, 6, 7, 2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 5,
            5, 4, 5, 4, 5, 6, 7, 7, 5, 5, 6, 1, 0, 1, 2, 3, 3, 1, 1, 0, 1, 2, 1, 6, 7, 7, 5, 5, 4, 5, 4, 5, 6, 5, 5, 1,
            1, 0, 1, 0, 1, 2, 3, 3, 1, 1, 2, 3, 4, 3, 2, 1, 1, 3, 3, 4, 3, 2, 3, 12, 12, 16, 12, 8, 12, 14, 14, 16, 14,
            16, 14, 12, 10, 10, 14, 14, 12, 10, 8, 10, 12, 14, 14, 10, 10, 8, 10, 12, 10, 15, 15, 16, 15, 16, 15, 14, 13,
            13, 15, 15, 14, 15, 16, 15, 14, 13, 13, 15, 15, 16, 15, 14, 15, 10, 9, 9, 11, 11, 12, 11, 12, 11, 10, 11, 11,
            13, 13, 12, 13, 12, 13, 14, 15, 15, 13, 13, 14, 9, 8, 9, 10, 11, 11, 9, 9, 8, 9, 10, 9, 14, 15, 15, 13, 13,
            12, 13, 12, 13, 14, 13, 13, 9, 9, 8, 9, 8, 9, 10, 11, 11, 9, 9, 10, 11, 12, 11, 10, 9, 9, 11, 11, 12, 11, 10,
            11
        };

        private static byte[] backslashYIndex = new byte[]
        {
            0, 0, 0, 8, 8, 8, 16, 16, 16, 0, 4, 4, 4, 4, 8, 4, 6, 6, 2, 2, 0, 2, 0, 2, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 2, 2,
            6, 6, 4, 6, 7, 7, 5, 5, 4, 5, 4, 5, 6, 5, 5, 1, 1, 0, 1, 0, 1, 2, 3, 3, 1, 1, 2, 1, 0, 1, 2, 3, 3, 1, 1, 0,
            1, 2, 1, 2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 7, 7, 8, 7, 8, 7, 6, 5, 5, 7, 7, 6, 7, 8, 7, 6, 5, 5, 7, 7, 8,
            7, 6, 7, 2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 5, 5, 4, 5, 4, 5, 6, 7, 7, 5, 5, 6, 0, 4, 4, 4, 4, 8, 4, 6, 6,
            2, 2, 0, 2, 0, 2, 4, 2, 2, 6, 6, 8, 6, 8, 6, 4, 2, 2, 6, 6, 4, 6, 7, 7, 5, 5, 4, 5, 4, 5, 6, 5, 5, 1, 1, 0,
            1, 0, 1, 2, 3, 3, 1, 1, 2, 1, 0, 1, 2, 3, 3, 1, 1, 0, 1, 2, 1, 2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 7, 7, 8,
            7, 8, 7, 6, 5, 5, 7, 7, 6, 7, 8, 7, 6, 5, 5, 7, 7, 8, 7, 6, 7, 2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 5, 5, 4,
            5, 4, 5, 6, 7, 7, 5, 5, 6, 8, 12, 12, 12, 12, 16, 12, 14, 14, 10, 10, 8, 10, 8, 10, 12, 10, 10, 14, 14, 16,
            14, 16, 14, 12, 10, 10, 14, 14, 12, 14, 15, 15, 13, 13, 12, 13, 12, 13, 14, 13, 13, 9, 9, 8, 9, 8, 9, 10, 11,
            11, 9, 9, 10, 9, 8, 9, 10, 11, 11, 9, 9, 8, 9, 10, 9, 10, 9, 9, 11, 11, 12, 11, 12, 11, 10, 11, 11, 15, 15,
            16, 15, 16, 15, 14, 13, 13, 15, 15, 14, 15, 16, 15, 14, 13, 13, 15, 15, 16, 15, 14, 15, 10, 9, 9, 11, 11, 12,
            11, 12, 11, 10, 11, 11, 13, 13, 12, 13, 12, 13, 14, 15, 15, 13, 13, 14, 8, 12, 12, 12, 12, 16, 12, 14, 14,
            10, 10, 8, 10, 8, 10, 12, 10, 10, 14, 14, 16, 14, 16, 14, 12, 10, 10, 14, 14, 12, 14, 15, 15, 13, 13, 12, 13,
            12, 13, 14, 13, 13, 9, 9, 8, 9, 8, 9, 10, 11, 11, 9, 9, 10, 9, 8, 9, 10, 11, 11, 9, 9, 8, 9, 10, 9, 10, 9, 9,
            11, 11, 12, 11, 12, 11, 10, 11, 11, 15, 15, 16, 15, 16, 15, 14, 13, 13, 15, 15, 14, 15, 16, 15, 14, 13, 13,
            15, 15, 16, 15, 14, 15, 10, 9, 9, 11, 11, 12, 11, 12, 11, 10, 11, 11, 13, 13, 12, 13, 12, 13, 14, 15, 15, 13,
            13, 14
        };

        private static byte[] slashXIndex = new byte[]
        {
            0, 8, 16, 0, 8, 16, 0, 8, 16, 4, 0, 4, 4, 4, 8, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 6, 6, 8, 6, 8, 6, 4, 2, 2,
            6, 6, 4, 1, 0, 1, 2, 3, 3, 1, 1, 0, 1, 2, 1, 6, 7, 7, 5, 5, 4, 5, 4, 5, 6, 5, 5, 1, 1, 0, 1, 0, 1, 2, 3, 3,
            1, 1, 2, 3, 4, 3, 2, 1, 1, 3, 3, 4, 3, 2, 3, 7, 7, 8, 7, 8, 7, 6, 5, 5, 7, 7, 6, 7, 8, 7, 6, 5, 5, 7, 7, 8,
            7, 6, 7, 2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 5, 5, 4, 5, 4, 5, 6, 7, 7, 5, 5, 6, 12, 8, 12, 12, 12, 16, 10,
            8, 10, 12, 14, 14, 10, 10, 8, 10, 12, 10, 14, 14, 16, 14, 16, 14, 12, 10, 10, 14, 14, 12, 9, 8, 9, 10, 11,
            11, 9, 9, 8, 9, 10, 9, 14, 15, 15, 13, 13, 12, 13, 12, 13, 14, 13, 13, 9, 9, 8, 9, 8, 9, 10, 11, 11, 9, 9,
            10, 11, 12, 11, 10, 9, 9, 11, 11, 12, 11, 10, 11, 15, 15, 16, 15, 16, 15, 14, 13, 13, 15, 15, 14, 15, 16, 15,
            14, 13, 13, 15, 15, 16, 15, 14, 15, 10, 9, 9, 11, 11, 12, 11, 12, 11, 10, 11, 11, 13, 13, 12, 13, 12, 13, 14,
            15, 15, 13, 13, 14, 4, 0, 4, 4, 4, 8, 2, 0, 2, 4, 6, 6, 2, 2, 0, 2, 4, 2, 6, 6, 8, 6, 8, 6, 4, 2, 2, 6, 6, 4,
            1, 0, 1, 2, 3, 3, 1, 1, 0, 1, 2, 1, 6, 7, 7, 5, 5, 4, 5, 4, 5, 6, 5, 5, 1, 1, 0, 1, 0, 1, 2, 3, 3, 1, 1, 2,
            3, 4, 3, 2, 1, 1, 3, 3, 4, 3, 2, 3, 7, 7, 8, 7, 8, 7, 6, 5, 5, 7, 7, 6, 7, 8, 7, 6, 5, 5, 7, 7, 8, 7, 6, 7,
            2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 5, 5, 4, 5, 4, 5, 6, 7, 7, 5, 5, 6, 12, 8, 12, 12, 12, 16, 10, 8, 10, 12,
            14, 14, 10, 10, 8, 10, 12, 10, 14, 14, 16, 14, 16, 14, 12, 10, 10, 14, 14, 12, 9, 8, 9, 10, 11, 11, 9, 9, 8,
            9, 10, 9, 14, 15, 15, 13, 13, 12, 13, 12, 13, 14, 13, 13, 9, 9, 8, 9, 8, 9, 10, 11, 11, 9, 9, 10, 11, 12, 11,
            10, 9, 9, 11, 11, 12, 11, 10, 11, 15, 15, 16, 15, 16, 15, 14, 13, 13, 15, 15, 14, 15, 16, 15, 14, 13, 13, 15,
            15, 16, 15, 14, 15, 10, 9, 9, 11, 11, 12, 11, 12, 11, 10, 11, 11, 13, 13, 12, 13, 12, 13, 14, 15, 15, 13, 13,
            14
        };

        private static byte[] slashYIndex = new byte[]
        {
            0, 0, 0, 8, 8, 8, 16, 16, 16, 0, 4, 4, 4, 8, 4, 4, 6, 6, 2, 2, 0, 2, 0, 2, 4, 2, 2, 6, 8, 6, 4, 2, 2, 6, 6, 8,
            6, 4, 6, 6, 7, 7, 5, 5, 4, 5, 4, 5, 6, 5, 5, 1, 1, 0, 1, 0, 1, 2, 3, 3, 1, 1, 2, 1, 0, 1, 2, 3, 3, 1, 1, 0,
            1, 2, 1, 2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 7, 8, 7, 6, 5, 5, 7, 7, 8, 7, 6, 7, 2, 1, 1, 3, 3, 4, 3, 4, 3,
            2, 3, 3, 7, 7, 8, 7, 8, 7, 6, 5, 5, 7, 7, 6, 5, 4, 5, 6, 7, 7, 5, 5, 4, 5, 6, 5, 0, 4, 4, 4, 8, 4, 4, 6, 6,
            2, 2, 0, 2, 0, 2, 4, 2, 2, 6, 8, 6, 4, 2, 2, 6, 6, 8, 6, 4, 6, 6, 7, 7, 5, 5, 4, 5, 4, 5, 6, 5, 5, 1, 1, 0,
            1, 0, 1, 2, 3, 3, 1, 1, 2, 1, 0, 1, 2, 3, 3, 1, 1, 0, 1, 2, 1, 2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 7, 8, 7,
            6, 5, 5, 7, 7, 8, 7, 6, 7, 2, 1, 1, 3, 3, 4, 3, 4, 3, 2, 3, 3, 7, 7, 8, 7, 8, 7, 6, 5, 5, 7, 7, 6, 5, 4, 5,
            6, 7, 7, 5, 5, 4, 5, 6, 5, 8, 12, 12, 12, 16, 12, 12, 14, 14, 10, 10, 8, 10, 8, 10, 12, 10, 10, 14, 16, 14,
            12, 10, 10, 14, 14, 16, 14, 12, 14, 14, 15, 15, 13, 13, 12, 13, 12, 13, 14, 13, 13, 9, 9, 8, 9, 8, 9, 10, 11,
            11, 9, 9, 10, 9, 8, 9, 10, 11, 11, 9, 9, 8, 9, 10, 9, 10, 9, 9, 11, 11, 12, 11, 12, 11, 10, 11, 11, 15, 16,
            15, 14, 13, 13, 15, 15, 16, 15, 14, 15, 10, 9, 9, 11, 11, 12, 11, 12, 11, 10, 11, 11, 15, 15, 16, 15, 16, 15,
            14, 13, 13, 15, 15, 14, 13, 12, 13, 14, 15, 15, 13, 13, 12, 13, 14, 13, 8, 12, 12, 12, 16, 12, 12, 14, 14,
            10, 10, 8, 10, 8, 10, 12, 10, 10, 14, 16, 14, 12, 10, 10, 14, 14, 16, 14, 12, 14, 14, 15, 15, 13, 13, 12, 13,
            12, 13, 14, 13, 13, 9, 9, 8, 9, 8, 9, 10, 11, 11, 9, 9, 10, 9, 8, 9, 10, 11, 11, 9, 9, 8, 9, 10, 9, 10, 9, 9,
            11, 11, 12, 11, 12, 11, 10, 11, 11, 15, 16, 15, 14, 13, 13, 15, 15, 16, 15, 14, 15, 10, 9, 9, 11, 11, 12, 11,
            12, 11, 10, 11, 11, 15, 15, 16, 15, 16, 15, 14, 13, 13, 15, 15, 14, 13, 12, 13, 14, 15, 15, 13, 13, 12, 13,
            14, 13
        };

        private readonly IPlateTilePyramid _plateTiles;
        private readonly WwtOptions _options;

        public HiriseDem2(IPlateTilePyramid plateTiles, WwtOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public async Task<Stream> GetMolaDemTileStreamAsync(int level, int x, int y, CancellationToken token)
        {
            float[] dataOut = await GetMolaDemTileAsync(level, x, y, token);
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);

            for (int i = 0; i < 513; i++)
            {
                bw.Write(dataOut[i]);
            }
            bw.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public async Task<Stream> MergeMolaDemTileStream(int level, int x, int y, Stream tile, CancellationToken token = default)
        {
            float[] dataIn = ReadDemStream(tile);

            float[] dataOut = await GetMolaDemTileAsync(level, x, y, token);

            for (int i = 0; i < 513; i++)
            {
                if (dataIn[i] != 0 && dataIn[i] != float.MinValue)
                {
                    dataOut[i] = dataIn[i];
                }
            }

            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);

            for (int i = 0; i < 513; i++)
            {
                bw.Write(dataOut[i]);
            }
            bw.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        private async ValueTask<float[]> GetMolaDemTileAsync(int level, int xx, int yy, CancellationToken token)
        {
            float[] data = await GetMolaDEMTileRawAsync(level, xx, yy, token);

            if (data == null)
            {
                float[] parent = await GetMolaDemTileAsync(level - 1, xx / 2, yy / 2, token);

                bool backslash = ComputeBackslash(level, xx, yy);

                // convert to matrix order from vertex order
                parent = MakeDemArray(parent, backslash);

                int offsetX = ((xx % 2) == 1 ? 8 : 0);
                int offsetY = ((yy % 2) == 0 ? 8 : 0);


                float[] demArray = new float[17 * 17];
                // Interpolate accross 
                for (int y = 0; y < 17; y += 2)
                {
                    bool copy = true;
                    for (int x = 0; x < 17; x++)
                    {
                        if (copy)
                        {
                            demArray[(16 - y) * 17 + x] = GetDemSample(parent, (x / 2) + offsetX, (y / 2) + offsetY);
                        }
                        else
                        {
                            demArray[(16 - y) * 17 + x] =
                                (
                                    (
                                        GetDemSample(parent, (x / 2) + offsetX, (y / 2) + offsetY) +
                                        GetDemSample(parent, ((x / 2) + offsetX) + 1, (y / 2) + offsetY)
                                        ) / 2);
                        }
                        copy = !copy;

                    }
                }
                // Interpolate down
                for (int y = 1; y < 17; y += 2)
                {
                    for (int x = 0; x < 17; x++)
                    {

                        demArray[(16 - y) * 17 + x] =
                            (
                                (
                                    GetDemSample(demArray, x, y - 1) +
                                    GetDemSample(demArray, x, y + 1)
                                    ) / 2);

                    }
                }

                // Convert the dem array back to the arranged DEM list thu slash/backslash mapping tables


                data = new float[513];
                for (int i = 0; i < 513; i++)
                {
                    if (backslash)
                    {
                        data[i] = demArray[backslashXIndex[i] + backslashYIndex[i] * 17];
                    }
                    else
                    {
                        data[i] = demArray[slashXIndex[i] + slashYIndex[i] * 17];
                    }
                }
            }

            return data;
        }

        private static bool ComputeBackslash(int level, int x, int y)
        {
            int width = (int)((Math.Pow(2, level + 1) - 1) / 2);

            if (x >= width && y < width)
            {
                return true;
            }

            if (x < width && y >= width)
            {
                return true;
            }

            return false;
        }

        private static float[] MakeDemArray(float[] data, bool backslash)
        {
            float[] dataOut = new float[17 * 17];
            for (int i = 0; i < 513; i++)
            {
                if (backslash)
                {
                    dataOut[((backslashYIndex[i])) * 17 + backslashXIndex[i]] = data[i];
                }
                else
                {
                    dataOut[(slashYIndex[i]) * 17 + slashXIndex[i]] = data[i];
                }
            }

            return dataOut;
        }

        private static float GetDemSample(float[] demArray, int x, int y)
        {
            return demArray[(16 - y) * 17 + x];
        }

        private async ValueTask<float[]> GetMolaDEMTileRawAsync(int level, int x, int y, CancellationToken token)
        {
            using (var stream = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "marsToastDem.plate", -1, level, x, y, token))
            {
                if (stream != null)
                {
                    return ReadDemStream(stream);
                }
                return null;
            }
        }

        private static float[] ReadDemStream(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            float[] data = new float[513];

            for (int i = 0; i < 513; i++)
            {
                data[i] = br.ReadSingle();
            }

            br.Close();
            return data;
        }
    }
}