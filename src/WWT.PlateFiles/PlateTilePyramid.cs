using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;

/* Jonathan Fay wrote this, except for the bits polluted by Dinoj, which are between <dinoj>...</dinoj> tags
*  The header information in the .plate files is being upgraded.
 * The original versiof plate files (such as those for the DSS tiles, which are of the form N177.plate etc 
 *   have their first eight bytes free but empty. One had to know the number of levels they had (7 for DSS) 
 *   in order to use them. This code will always have to be able to read those files for legacy purposes.
 * The current version of .plate files has the first eight bytes used - the first four contain a magic number 
 *   that says that they are this version of plate files and the next four bytes contain the number of levels.
 * If you request a nonexistent L-X-Y tile, null is returned. Note that this sometimes happens on a level
 *   where tiles are present, since the overall images contained in the .plate file may have side length
 *   a power of 2.
 * 
 * Example of the first eight bytes (in Hex) : 43AD697E03000000
 * 
 * The 43AD697E is the magic number (really 7E69AD43) and the number of levels (00000003) is three.
 * Yes, we stored them least significant byte first.
*/

namespace WWTWebservices
{
    public class PlateTilePyramid : IDisposable
    {
        string filename;
        private FileStream _readStream;
        private bool _disposed = false;

        public string FileName
        {
            get { return filename; }
        }
        int levels;

        public int Levels
        {
            get { return levels; }
        }
        uint currentOffset = 0;
        LevelInfo[] levelMap;

        // <dinoj>

        const uint dotPlateFileTypeNumber = 2120854851; // 7E69AD43 in hex
        /// magic number is ceil(0.9876 * 2^31) = 0111 1110 0110 1001 1010 1101 0100 0011 in binary
        /// this identifies that this plate file has useful header information

        public PlateTilePyramid(string filename)
        {
            int L = -1;
            if (GetLevelCount(filename, out L))
            {
                this.filename = filename;
                this.levels = L;
            }
            else
            {
                this.filename = "";      // UNCLEAR WHAT TO SET THIS TO
                this.levels = -1;        // UNCLEAR WHAT TO SET THIS TO
            }
        }
        // </dinoj>

        public PlateTilePyramid(string filename, int levels)
        {
            this.filename = filename;
            this.levels = levels;
        }

        FileStream fileStream = null;

        public void Create()
        {
            levelMap = new LevelInfo[levels];

            for (int i = 0; i < levels; i++)
            {
                levelMap[i] = new LevelInfo(i);
            }
            fileStream = File.Open(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            WriteHeaders();
            currentOffset = HeaderSize;
            fileStream.Seek(currentOffset, SeekOrigin.Begin);

        }

        // would be nice to have a version of AddFile that has as its first argument a Bitmap or Stream
        public void AddFile(string inputFilename, int level, int x, int y)
        {
            long start = fileStream.Seek(0, SeekOrigin.End);
            byte[] buf = null;

            using (FileStream fs = File.Open(inputFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int len = (int)fs.Length;
                buf = new byte[fs.Length];

                levelMap[level].fileMap[x, y].start = (uint)start;
                levelMap[level].fileMap[x, y].size = (uint)len;

                fs.Read(buf, 0, len);
                fileStream.Write(buf, 0, len);
                fs.Close();
            }
        }


        public void AddStream(Stream inputStream, int level, int x, int y)
        {
            // closes inputStream once read
            // added by Dinoj, debugged by Jonathan
            // TODO: add a while loop so that it can handle very long streams
            // (for our 256 x 256 bitmaps it's fine)
            long start = fileStream.Seek(0, SeekOrigin.End);
            byte[] buf = null;

            int len = (int)inputStream.Length;
            buf = new byte[inputStream.Length];

            levelMap[level].fileMap[x, y].start = (uint)start;
            levelMap[level].fileMap[x, y].size = (uint)len;
            inputStream.Seek(0, SeekOrigin.Begin);
            int lenRead = inputStream.Read(buf, 0, len);
            fileStream.Write(buf, 0, len);
            inputStream.Close();
        }

        public void UpdateHeaderAndClose()
        {
            if (fileStream != null)
            {
                WriteHeaders();
                fileStream.Close();
                fileStream = null;
            }
        }
        // <dinoj>
        static bool HasUsefulHeaders(string plateFileName)
        {
            // returns true if plateFileName has the magic number identifying this as a .plate file
            int L = -1;
            return GetLevelCount(plateFileName, out L);
        }

        public static bool GetLevelCount(string plateFileName, out int L)
        {
            // Returns true if plateFileName has the magic number identifying this as a .plate file
            // Also returns the number of levels in the .plate file. 
            //   This is 10 by default i.e. if no headers are found.
            L = 10; // 
            bool hasHeadersWithInfo = false;
            if (File.Exists(plateFileName))
            {
                using (FileStream fs = new FileStream(plateFileName, FileMode.Open, FileAccess.Read))
                {
                    if (fs != null)
                    {
                        uint FirstFourBytes, SecondFourBytes;
                        FirstFourBytes = GetNodeInfo(fs, 0, out SecondFourBytes);
                        if (FirstFourBytes == dotPlateFileTypeNumber)
                        {
                            L = (int)SecondFourBytes;
                            hasHeadersWithInfo = true;
                        }
                    }
                    fs.Close();
                }
            }
            return hasHeadersWithInfo;
        }
        //  </dinoj>

        private void WriteHeaders()
        {
            //  <dinoj>
            uint L = (uint)levels;
            byte[] buffer = new byte[8];
            buffer[0] = (byte)(dotPlateFileTypeNumber % 256);
            buffer[1] = (byte)((dotPlateFileTypeNumber >> 8) % 256);
            buffer[2] = (byte)((dotPlateFileTypeNumber >> 16) % 256);
            buffer[3] = (byte)((dotPlateFileTypeNumber >> 24) % 256);
            buffer[4] = (byte)L;
            buffer[5] = (byte)(L >> 8);
            buffer[6] = (byte)(L >> 16);
            buffer[7] = (byte)(L >> 24);

            fileStream.Write(buffer, 0, 8);
            //  </dinoj>

            uint currentSeek = 8;
            foreach (LevelInfo li in levelMap)
            {
                int count = (int)Math.Pow(2, li.level);
                for (int y = 0; y < count; y++)
                {
                    for (int x = 0; x < count; x++)
                    {
                        SetNodeInfo(fileStream, currentSeek, li.fileMap[x, y].start, li.fileMap[x, y].size);
                        currentSeek += 8;
                    }
                }
            }
        }

        uint HeaderSize
        {
            get
            {
                return GetFileIndexOffset(levels, 0, 0);
            }
        }



        static public uint GetFileIndexOffset(int level, int x, int y)
        {
            uint offset = 8;
            for (uint i = 0; i < level; i++)
            {
                offset += (uint)(Math.Pow(2, i * 2) * 8);
            }

            offset += (uint)(y * Math.Pow(2, level) + x) * 8;

            return offset;

        }

        public Stream GetFileStream(int level, int x, int y)
        {
            if (filename.Length > 0 && File.Exists(filename) && levels > level)
            {
                return GetFileStream(filename, level, x, y);
            }
            return null;
        }

        public static async Task<Stream> GetImageStreamAsync(Stream f, int level, int x, int y, CancellationToken token)
        {
            var offset = GetFileIndexOffset(level, x, y);
            f.Seek(offset, SeekOrigin.Begin);

            var (start, length) = await GetNodeInfoAsync(f, offset, token).ConfigureAwait(false);

            return new StreamSlice(f, start, length);
        }

        static public Stream GetFileStream(string filename, int level, int x, int y)
        {
            uint offset = GetFileIndexOffset(level, x, y);
            uint start;

            MemoryStream ms = null;
            using (FileStream f = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                f.Seek(offset, SeekOrigin.Begin);
                start = GetNodeInfo(f, offset, out var length);

                byte[] buffer = new byte[length];
                f.Seek(start, SeekOrigin.Begin);
                f.Read(buffer, 0, (int)length);
                ms = new MemoryStream(buffer);
                f.Close();
            }
            return ms;
        }

        public static async ValueTask<(uint start, uint length)> GetNodeInfoAsync(Stream fs, uint offset, CancellationToken token)
        {
            Byte[] buf = new Byte[8];
            fs.Seek(offset, SeekOrigin.Begin);
            await fs.ReadAsync(buf, 0, 8, token).ConfigureAwait(false);

            var length = (uint)(buf[4] + (buf[5] << 8) + (buf[6] << 16) + (buf[7] << 24));
            var start = (uint)((buf[0] + (buf[1] << 8) + (buf[2] << 16) + (buf[3] << 24)));

            return (start, length);
        }

        public static uint GetNodeInfo(Stream fs, uint offset, out uint length)
        {
            Byte[] buf = new Byte[8];
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(buf, 0, 8);

            length = (uint)(buf[4] + (buf[5] << 8) + (buf[6] << 16) + (buf[7] << 24));

            return (uint)((buf[0] + (buf[1] << 8) + (buf[2] << 16) + (buf[3] << 24)));
        }

        static public void SetNodeInfo(FileStream fs, uint offset, uint start, uint length)
        {
            Byte[] buf = new Byte[8];
            buf[0] = (byte)start;
            buf[1] = (byte)(start >> 8);
            buf[2] = (byte)(start >> 16);
            buf[3] = (byte)(start >> 24);
            buf[4] = (byte)length;
            buf[5] = (byte)(length >> 8);
            buf[6] = (byte)(length >> 16);
            buf[7] = (byte)(length >> 24);

            fs.Seek(offset, SeekOrigin.Begin);
            fs.Write(buf, 0, 8);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        public void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    if (_readStream != null)
                        _readStream.Dispose();
                    _readStream = null;
                }
                this._disposed = true;
            }
        }
    }


    public class LevelInfo
    {
        public int level;
        public NodeInfo[,] fileMap;

        public LevelInfo(int level)
        {
            this.level = level;
            fileMap = new NodeInfo[(int)Math.Pow(2, level), (int)Math.Pow(2, level)];
        }
    }

    public struct NodeInfo
    {
        public uint start;
        public uint size;
        public string filename;
    }
}
