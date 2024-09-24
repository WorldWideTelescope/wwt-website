#nullable disable

//------------------------------------------------------------------------------
// <copyright file="DemCodec.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <createdby>bretm</createdby><creationdate>2005-11-29</creationdate>
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Maps
{
    /// <summary>
    /// Provides serialization and deserialization of digital elevation maps.
    /// Based on $\VirtualEarth\Main\Metropolis\Applications\DEMPipeline\DemCompress\DemCodec\DemCodec.cs
    /// </summary>
    public sealed class DemCodec
    {
        #region Fields

        const byte DemStyleWDP = 4;
        #endregion

        #region public Members

        /// <summary>
        /// Decompress a stream into a Dem tile.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async ValueTask<DemTile> DecompressAsync(Stream input, CancellationToken token)
        {
            // validate input arguments
            if (input == null || !input.CanRead)
                return null;

            try
            {
                if (input.ReadByte() == DemStyleWDP)
                {
                    return await DecodeAsync(input, token);
                }
            }
            catch (IOException)
            { }

            return null;
        }
        #endregion

        #region Private Members

        private static async ValueTask<DemTile> DecodeAsync(Stream input, CancellationToken token)
        {
            // read meta info 
            byte[] b = new byte[4 * sizeof(int)];
            await input.ReadAsync(b, 0, b.Length, token);

            int width = BitConverter.ToInt32(b, 0);
            if (width < 1 || width > 1000)
                return null;

            int height = BitConverter.ToInt32(b, sizeof(int));
            if (height < 1 || height > 1000)
                return null;

            int avg = BitConverter.ToInt32(b, 2 * sizeof(int));
            if (avg < short.MinValue || avg > short.MaxValue)
                return null;

            int length = BitConverter.ToInt32(b, 3 * sizeof(int));
            if (length < 0 || length > 200000)
                return null;

            // read compressed data
            byte[] data = new byte[length];
            int offset = 0;
            while (offset < length)
            {
                int actual = await input.ReadAsync(data, offset, length - offset, token);
                if (actual <= 0 || actual > length - offset)
                {
                    return null;
                }
                offset += actual;
            }

            short[,] pixels = new HDPhotoDecoder().Decode(data);

            if (pixels == null)
            {
                return null;
            }

            try
            {
                // read error-corrections
                BitReader r = new BitReader(input);
                for (int row = 0; row < height; row++)
                    for (int col = 0; col < width; col++)
                        if ((row & 0xF) == 0 || (col & 0xF) == 0)
                        {
                            // offset = row * width + col;
                            short err = ReadUnaryCorrection(r);
                            pixels[row, col] += err;
                        }
            }
            catch (EndOfStreamException)
            {
                return null;
            }

            // create tile
            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                    pixels[row, col] += (short)avg;
            DemTile tile = new DemTile(pixels);

            return tile;
        }

        private static short ReadUnaryCorrection(BitReader r)
        {
            if (!r.ReadBit())
                return 0;

            short err = 1;
            while (true)
            {
                if (!r.ReadBit())
                    break;
                err++;
            }

            if (r.ReadBit())
                err = (short)-err;

            return err;
        }
        #endregion
    }
}
