using System;
using WWTWebservices;

namespace WWT.Providers
{
    public abstract partial class DemMars : RequestProvider
    {
        public UInt32 ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }
    }
}
