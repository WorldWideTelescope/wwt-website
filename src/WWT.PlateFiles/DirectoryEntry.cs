#nullable disable

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace WWT.PlateFiles
{
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
