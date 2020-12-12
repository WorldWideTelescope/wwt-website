#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using WWT.PlateFiles;

namespace WWT.PlateFiles
{
    public class PlateFile2
    {
        Stream dataStream;

        public PlateFile2(string filename, int filecount)
        {
            dataStream = File.Open(filename, FileMode.Create);
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
            dataStream.Write(headerData, 0, headerData.Length);
            dataStream.Seek(header.NextFreeDirectoryEntry + Marshal.SizeOf(entry) * header.HashBuckets, SeekOrigin.Begin);
            dataStream.WriteByte(42);

            dataStream.Seek(header.FirstDirectoryEntry, SeekOrigin.Begin);
            entry = new DirectoryEntry();
            entry.size = (uint)header.FreeEntries * (uint)Marshal.SizeOf(entry);
            entry.location = header.FirstDirectoryEntry;
            byte[] entryData = GetEntryBytes();
            dataStream.Write(entryData, 0, entryData.Length);

            dataStream.Seek(0, SeekOrigin.Begin);
        }

        public void Close()
        {
            if (dataStream != null)
            {
                dataStream.Close();
                dataStream = null;
            }
        }

        public PlateFile2(Stream stream)
        {
            dataStream = stream;
            ReadHeader();
        }

        public PlateFile2(string filename)
        {
            dataStream = File.Open(filename, FileMode.Open);
            ReadHeader();
        }

        public PlateFile2(string filename, bool readOnly)
        {
            dataStream = File.Open(filename, FileMode.Open, readOnly ? FileAccess.Read : FileAccess.ReadWrite, FileShare.Read);
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
                    header.FirstDirectoryEntry = dataStream.Seek(0, SeekOrigin.End);
                    dataStream.Write(entryData, 0, entryData.Length);

                    // Allocate a new 8k entries
                    header.FreeEntries = 8192;
                    header.NextFreeDirectoryEntry = dataStream.Seek(0, SeekOrigin.End);
                    dataStream.Seek(8192 * Marshal.SizeOf(entry), SeekOrigin.End);
                    dataStream.WriteByte(42);
                }


                long position = dataStream.Seek(0, SeekOrigin.End);
                entry = new DirectoryEntry(tag, level, x, y, position, (UInt32)fi.Length);

                UInt32 index = entry.ComputeHash() % (UInt32)header.HashBuckets;
                byte[] buf = null;

                // Copy the input file stream to the end of the file
                using (FileStream ifs = File.Open(inputFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int len = (int)fi.Length;
                    buf = new byte[fi.Length];

                    ifs.Read(buf, 0, len);
                    dataStream.Write(buf, 0, len);
                    ifs.Close();
                }

                long hep = GetHashEntryPosition(index);
                entry.NextEntryInChain = hep;

                // Update the Hash Table
                WriteHashEntryPosition(index, header.NextFreeDirectoryEntry);

                // Write the directory entry
                dataStream.Seek(header.NextFreeDirectoryEntry, SeekOrigin.Begin);

                entryData = GetEntryBytes();
                dataStream.Write(entryData, 0, entryData.Length);

                // Update Header
                header.FileCount++;
                header.NextFreeDirectoryEntry += Marshal.SizeOf(entry);
                header.FreeEntries--;



                // Write it back to file
                Byte[] headerData = GetHeaderBytes();
                dataStream.Seek(0, SeekOrigin.Begin);
                dataStream.Write(headerData, 0, headerData.Length);

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
            dataStream.Read(buf, 0, (int)buf.Length);
            de = GetDirectoryEntry(header.FirstDirectoryEntry);
            buf = new byte[de.size];
            dataStream.Read(buf, 0, (int)de.size);

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
                        dataStream.Seek(curPos, SeekOrigin.Begin);
                        de.Write(dataStream);
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

        public static Stream GetImageStream(Stream stream, int tag, int level, int x, int y)
        {
            PlateFile2 plate = new PlateFile2(stream);

            Stream result = plate.GetFileStream(tag, level, x, y);

            return result;
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
                    return StreamSlice.Create(dataStream, de.location, de.size);
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
                dataStream.Seek(den.location, SeekOrigin.Begin);
                dataStream.Read(buffer, 0, (int)den.size);
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
            dataStream.Seek(Marshal.SizeOf(header) + (hashindex * 8), SeekOrigin.Begin);

            return ReadLong();
        }

        public long ReadLong()
        {
            Byte[] data = new Byte[8];
            int count = dataStream.Read(data, 0, 8);

            return BitConverter.ToInt64(data, 0);
        }

        public void WriteHashEntryPosition(UInt32 hashindex, long position)
        {
            if (hashindex >= header.HashBuckets)
            {
                throw (new IndexOutOfRangeException());
            }
            dataStream.Seek(Marshal.SizeOf(header) + (hashindex * 8), SeekOrigin.Begin);

            WriteLong(position);
        }

        public void WriteLong(long val)
        {
            Byte[] data = BitConverter.GetBytes(val);
            dataStream.Write(data, 0, 8);
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
            dataStream.Seek(0, SeekOrigin.Begin);
            dataStream.Read(buffer, 0, buffer.Length);
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
            dataStream.Seek(pos, SeekOrigin.Begin);
            byte[] data = new byte[Marshal.SizeOf(entry)];
            dataStream.Read(data, 0, data.Length);

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
}
