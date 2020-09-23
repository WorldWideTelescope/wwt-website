namespace WWTWebservices
{
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
}