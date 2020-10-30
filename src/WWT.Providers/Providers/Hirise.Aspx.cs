using WWTWebservices;

namespace WWT.Providers
{
    public abstract partial class HiRise : RequestProvider
    {
        public uint ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }
    }
}