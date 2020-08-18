using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace PlateFile2
{

    public class PlateFile2
    {
        Stream fileStream;

        public PlateFile2(string filename, int filecount)
        {
            fileStream = File.Open(filename, FileMode.Create);
            //Initialize the header
            header.Signature = 0x17914242;
            header.HashBuckets = NextPowerOfTwo(filecount);
            header.HashTableLocation = Marshal.SizeOf(header);
            header.FirstDirectoryEntry = header.HashTableLocation + header.HashBuckets * 8;
            header.NextFreeDirectoryEntry = header.FirstDirectoryEntry + Marshal.SizeOf(entry);
            header.FileCount = 0;
            header.FreeEntries = header.HashBuckets - 1;

            //Write the header and the empty Hash area. O/S will zero the data
            Byte[] headerData = GetHeaderBytes();
            fileStream.Write(headerData, 0, headerData.Length);
            fileStream.Seek(header.NextFreeDirectoryEntry + Marshal.SizeOf(entry) * header.HashBuckets, SeekOrigin.Begin);
            fileStream.WriteByte(42);

            fileStream.Seek(header.FirstDirectoryEntry, SeekOrigin.Begin);
            entry = new DirectoryEntry();
            entry.size = (uint)header.FreeEntries * (uint)Marshal.SizeOf(entry);
            entry.location = header.FirstDirectoryEntry;
            byte[] entryData = GetEntryBytes();
            fileStream.Write(entryData, 0, entryData.Length);

            fileStream.Seek(0, SeekOrigin.Begin);
        }

        public void Close()
        {
            if (fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }
        }

        public PlateFile2(string filename)
        {
            fileStream = File.Open(filename, FileMode.Open);
            ReadHeader();
        }

        public PlateFile2(string filename, bool readOnly)
        {
            fileStream = File.Open(filename, FileMode.Open, readOnly ? FileAccess.Read : FileAccess.ReadWrite, FileShare.Read);
            ReadHeader();
        }

        public PlateFile2(Stream platefileStream, bool readOnly)
        {
            fileStream = platefileStream;
            ReadHeader();
        }

        int[] entries = new int[256];

        PlateHeader header;

        public PlateHeader Header
        {
            get { return header; }
            set { header = value; }
        }
        DirectoryEntry entry;

        public void AddFile(string inputFilename, int tag, int level, int x, int y)
        {
            FileInfo fi = new FileInfo(inputFilename);
            if (fi.Exists)
            {
                Byte[] entryData = null;

                // Check for empty directory and add new Directory Block
                if (header.FreeEntries == 0)
                {
                    // Write Directory Header Block
                    entry = new DirectoryEntry();
                    entry.size = 8192 * (uint)Marshal.SizeOf(entry);
                    entry.NextEntryInChain = header.FirstDirectoryEntry;
                    entryData = GetEntryBytes();
                    header.FirstDirectoryEntry = fileStream.Seek(0, SeekOrigin.End);
                    fileStream.Write(entryData, 0, entryData.Length);

                    // Allocate a new 8k entries
                    header.FreeEntries = 8192;
                    header.NextFreeDirectoryEntry = fileStream.Seek(0, SeekOrigin.End);
                    fileStream.Seek(8192 * Marshal.SizeOf(entry), SeekOrigin.End);
                    fileStream.WriteByte(42);
                }


                long position = fileStream.Seek(0, SeekOrigin.End);
                entry = new DirectoryEntry(tag, level, x, y, position, (UInt32)fi.Length);

                UInt32 index = entry.ComputeHash() % (UInt32)header.HashBuckets;
                byte[] buf = null;

                // Copy the input file stream to the end of the file
                using (FileStream ifs = File.Open(inputFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int len = (int)fi.Length;
                    buf = new byte[fi.Length];

                    ifs.Read(buf, 0, len);
                    fileStream.Write(buf, 0, len);
                    ifs.Close();
                }

                long hep = GetHashEntryPosition(index);
                entry.NextEntryInChain = hep;

                // Update the Hash Table
                WriteHashEntryPosition(index, header.NextFreeDirectoryEntry);

                // Write the directory entry
                fileStream.Seek(header.NextFreeDirectoryEntry, SeekOrigin.Begin);

                entryData = GetEntryBytes();
                fileStream.Write(entryData, 0, entryData.Length);

                // Update Header
                header.FileCount++;
                header.NextFreeDirectoryEntry += Marshal.SizeOf(entry);
                header.FreeEntries--;



                // Write it back to file
                Byte[] headerData = GetHeaderBytes();
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.Write(headerData, 0, headerData.Length);

                //todo check for file count and extend file when we get to end.


            }
        }

        public static void Optimize(string filename)
        {
            PlateFile2 plate = new PlateFile2(filename, false);
            plate.Optimize();
            plate.Close();
        }

        public void Optimize()
        {

            DirectoryEntry de = new DirectoryEntry(0, 0, 0, 0, 0, 0);
            // preload the directory & hashtable here to make sure reads are serial and in system cache

            List<List<DirectoryEntry>> list = new List<List<DirectoryEntry>>();
            byte[] buf = new byte[header.HashBuckets * 8];
            fileStream.Read(buf, 0, (int)buf.Length);
            de = GetDirectoryEntry(header.FirstDirectoryEntry);
            buf = new byte[de.size];
            fileStream.Read(buf, 0, (int)de.size);

            for (UInt32 i = 0; i < header.HashBuckets; i++)
            {
                List<DirectoryEntry> dirList = new List<DirectoryEntry>();
                long hep = GetHashEntryPosition(i);

                while (hep != 0)
                {
                    de = GetDirectoryEntry(hep);
                    dirList.Add(de);

                    hep = de.NextEntryInChain;
                }
                dirList.Sort();

                list.Add(dirList);

            }


            long curPos = header.FirstDirectoryEntry + Marshal.SizeOf(entry);
            long[] hashTable = new long[header.HashBuckets];
            for (int i = 0; i < header.HashBuckets; i++)
            {
                List<DirectoryEntry> dir = list[i];

                if (dir.Count > 0)
                {
                    hashTable[i] = curPos;
                    for (int j = 0; j < dir.Count; j++)
                    {
                        de = dir[j];
                        if (j == dir.Count - 1)
                        {
                            de.NextEntryInChain = 0;
                        }
                        else
                        {
                            de.NextEntryInChain = curPos + Marshal.SizeOf(entry);
                        }
                        fileStream.Seek(curPos, SeekOrigin.Begin);
                        de.Write(fileStream);
                        curPos += Marshal.SizeOf(entry);
                    }
                }

            }
            // Write table seperately to sequence writes
            for (int i = 0; i < header.HashBuckets; i++)
            {
                WriteHashEntryPosition((uint)i, hashTable[i]);
            }

        }

        public static Stream GetFileStream(string filename, int tag, int level, int x, int y)
        {
            PlateFile2 plate = new PlateFile2(filename, true);

            Stream stream = plate.GetFileStream(tag, level, x, y);

            plate.Close();

            return stream;

        }

        public static Stream GetFileStream(Stream plateStream, int tag, int level, int x, int y)
        {
            PlateFile2 plate = new PlateFile2(plateStream, true);

            Stream stream = plate.GetFileStream(tag, level, x, y);

            plate.Close();

            return stream;

        }


        public IEnumerable<DirectoryEntry> GetEntries()
        {
            DirectoryEntry de = new DirectoryEntry(0, 0, 0, 0, 0, 0);

            for (UInt32 i = 0; i < header.HashBuckets; i++)
            {
                long hep = GetHashEntryPosition(i);

                while (hep != 0)
                {
                    de = GetDirectoryEntry(hep);

                    yield return de;

                    hep = de.NextEntryInChain;
                }
            }
        }

        public IEnumerable<DirectoryEntry> GetDirectoryEntries()
        {
            DirectoryEntry de = new DirectoryEntry(0, 0, 0, 0, 0, 0);

            long hep = header.FirstDirectoryEntry;

            while (hep != 0)
            {
                de = GetDirectoryEntry(hep);
                de.location = hep;
                if (de.size != 0)
                {
                    yield return de;
                }
                hep = de.NextEntryInChain;
            }
        }

        public int GetLongestChain()
        {
            DirectoryEntry de = new DirectoryEntry(0, 0, 0, 0, 0, 0);
            int longest = 0;
            int current = 0;
            for (UInt32 i = 0; i < header.HashBuckets; i++)
            {
                long hep = GetHashEntryPosition(i);
                current = 0;
                while (hep != 0)
                {
                    current++;
                    de = GetDirectoryEntry(hep);

                    hep = de.NextEntryInChain;
                }
                if (current > longest)
                {
                    longest = current;
                }
            }

            return longest;
        }

        public string GetLongestChainID()
        {
            DirectoryEntry de = new DirectoryEntry(0, 0, 0, 0, 0, 0);
            int longest = 0;
            int current = 0;
            string id = "";
            for (UInt32 i = 0; i < header.HashBuckets; i++)
            {
                long hep = GetHashEntryPosition(i);
                current = 0;
                while (hep != 0)
                {
                    current++;
                    de = GetDirectoryEntry(hep);

                    hep = de.NextEntryInChain;
                }
                if (current > longest)
                {
                    longest = current;
                    id = i.ToString();
                }
            }

            return id;
        }

        public int GetTotalFiles()
        {
            DirectoryEntry de = new DirectoryEntry(0, 0, 0, 0, 0, 0);
            int total = 0;
            for (UInt32 i = 0; i < header.HashBuckets; i++)
            {
                long hep = GetHashEntryPosition(i);
                while (hep != 0)
                {
                    total++;
                    de = GetDirectoryEntry(hep);

                    hep = de.NextEntryInChain;
                }

            }

            return total;
        }

        public int GetReportedTotalFiles()
        {
            return header.FileCount;
        }

        public Stream GetFileStream(int tag, int level, int x, int y)
        {
            entry = new DirectoryEntry(tag, level, x, y, 0, 0);

            UInt32 index = entry.ComputeHash() % (UInt32)(header.HashBuckets);

            long hep = GetHashEntryPosition(index);

            DirectoryEntry de;


            while (hep != 0)
            {
                de = GetDirectoryEntry(hep);

                if ((de.x == x) && (de.y == y) && ((de.tag == tag) || (tag == -1)) && (de.level == level))
                {
                    MemoryStream ms = null;

                    byte[] buffer = new byte[de.size];
                    fileStream.Seek(de.location, SeekOrigin.Begin);
                    fileStream.Read(buffer, 0, (int)de.size);
                    ms = new MemoryStream(buffer);
                    return ms;
                }
                else
                {
                    hep = de.NextEntryInChain;
                }

            }
            return null;
        }

        public Stream GetFileStreamFullSearch(int tag, int level, int x, int y)
        {
            entry = new DirectoryEntry(tag, level, x, y, 0, 0);

            UInt32 index = entry.ComputeHash() % (UInt32)(header.HashBuckets);

            long hep = GetHashEntryPosition(index);

            DirectoryEntry de;
            DirectoryEntry den = new DirectoryEntry();

            while (hep != 0)
            {
                de = GetDirectoryEntry(hep);

                if ((de.x == x) && (de.y == y) && ((de.tag == tag) || (tag == -1)) && (de.level == level))
                {
                    if (den.tag < de.tag)
                    {
                        den = de;
                        if (tag != -1)
                        {
                            break;
                        }
                    }
                }

                hep = de.NextEntryInChain;

            }

            if (den.location != 0)
            {
                MemoryStream ms = null;

                byte[] buffer = new byte[den.size];
                fileStream.Seek(den.location, SeekOrigin.Begin);
                fileStream.Read(buffer, 0, (int)den.size);
                ms = new MemoryStream(buffer);
                return ms;
            }


            return null;
        }


        public long GetHashEntryPosition(UInt32 hashindex)
        {
            if (hashindex >= header.HashBuckets)
            {
                throw (new IndexOutOfRangeException());
            }
            fileStream.Seek(Marshal.SizeOf(header) + (hashindex * 8), SeekOrigin.Begin);

            return ReadLong();
        }

        public long ReadLong()
        {
            Byte[] data = new Byte[8];
            int count = fileStream.Read(data, 0, 8);

            return BitConverter.ToInt64(data, 0);
        }

        public void WriteHashEntryPosition(UInt32 hashindex, long position)
        {
            if (hashindex >= header.HashBuckets)
            {
                throw (new IndexOutOfRangeException());
            }
            fileStream.Seek(Marshal.SizeOf(header) + (hashindex * 8), SeekOrigin.Begin);

            WriteLong(position);
        }

        public void WriteLong(long val)
        {
            Byte[] data = BitConverter.GetBytes(val);
            fileStream.Write(data, 0, 8);
        }

        public string GetEntryString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (int i in entries)
            {
                sb.AppendLine(i.ToString());
            }
            return sb.ToString();
        }

        private byte[] GetHeaderBytes()
        {
            byte[] buffer = new byte[Marshal.SizeOf(header)];
            GCHandle hStruct = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(header, hStruct.AddrOfPinnedObject(), false);
            hStruct.Free();
            return buffer;
        }

        private void ReadHeader()
        {
            byte[] buffer = new byte[Marshal.SizeOf(header)];
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
            SetHeaderBytes(buffer);
        }

        private void SetHeaderBytes(byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            header = (PlateHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(PlateHeader));
            handle.Free();
        }

        private DirectoryEntry GetDirectoryEntry(long pos)
        {
            fileStream.Seek(pos, SeekOrigin.Begin);
            byte[] data = new byte[Marshal.SizeOf(entry)];
            fileStream.Read(data, 0, data.Length);

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            DirectoryEntry de = (DirectoryEntry)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DirectoryEntry));
            handle.Free();
            return de;
        }

        private byte[] GetEntryBytes()
        {
            byte[] buffer = new byte[Marshal.SizeOf(entry)];
            GCHandle hStruct = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(entry, hStruct.AddrOfPinnedObject(), false);
            hStruct.Free();
            return buffer;
        }

        private void SetEntryBytes(byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            entry = (DirectoryEntry)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DirectoryEntry));
            handle.Free();
        }

        private int NextPowerOfTwo(int val)
        {
            val--;
            val = (val >> 1) | val;
            val = (val >> 2) | val;
            val = (val >> 4) | val;
            val = (val >> 8) | val;
            val = (val >> 16) | val;
            val++;
            return val;
        }
    }

    public struct PlateHeader
    {
        public int Signature;
        public int HashBuckets;
        public int FileCount;
        public int FreeEntries;
        public long HashTableLocation;
        public long NextFreeDirectoryEntry;
        public long FirstDirectoryEntry;
    }

    public struct DirectoryEntry : IComparable
    {
        static MD5 md5Hash = MD5.Create();
        static Mutex md5Mut = new Mutex();

        public DirectoryEntry(int tag, int level, int x, int y, long location, UInt32 fileSize)
        {
            hash = 0;
            this.level = (byte)level;
            this.x = x;
            this.y = y;
            this.tag = tag;
            this.location = location;
            this.size = fileSize;
            this.NextEntryInChain = 0;
        }

        public UInt32 ComputeHash()
        {
            hash = ComputeHash(level, x, y);

            return hash;
        }

        public static UInt32 ComputeHash(int level, int x, int y)
        {
            byte[] input = new byte[9];
            input[0] = (byte)level;
            input[1] = (byte)x;
            input[2] = (byte)(x >> 8);
            input[3] = (byte)(x >> 16);
            input[4] = (byte)(x >> 24);
            input[5] = (byte)y;
            input[6] = (byte)(y >> 8);
            input[7] = (byte)(y >> 16);
            input[8] = (byte)(y >> 24);
            md5Mut.WaitOne();
            byte[] output = md5Hash.ComputeHash(input);
            md5Mut.ReleaseMutex();

            return (UInt32)output[0] + ((UInt32)output[1] << 8) + ((UInt32)output[2] << 16) + ((UInt32)output[3] << 24);
        }

        public void Write(Stream stream)
        {
            byte[] buffer = new byte[Marshal.SizeOf(this)];
            GCHandle hStruct = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(this, hStruct.AddrOfPinnedObject(), false);
            hStruct.Free();
            stream.Write(buffer, 0, buffer.Length);
        }

        public int tag;
        public int level;
        public int x;
        public int y;
        public UInt32 hash;
        public long location;
        public UInt32 size;
        public long NextEntryInChain;

        int IComparable.CompareTo(object obj)
        {
            if (obj is DirectoryEntry)
            {
                DirectoryEntry de = (DirectoryEntry)obj;

                if (de.x == x && de.y == y && de.level == level)
                {
                    if (de.tag < tag)
                    {
                        return -1;
                    }
                    else if (de.tag == tag)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }

                if (de.level < level)
                {
                    return 1;
                }
                else if (de.level > level)
                {
                    return -1;
                }
                else
                {
                    if (de.x < x)
                    {
                        return 1;
                    }
                    else if (de.x > x)
                    {
                        return -1;
                    }
                    else
                    {
                        if (de.y < y)
                        {
                            return 1;
                        }
                        else if (de.y > y)
                        {
                            return -1;
                        }
                        else
                        {
                            return 0;
                        }

                    }

                }
            }
            return -1;
        }
    }
}
