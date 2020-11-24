using System;

namespace WWT.PlateFiles
{
    public class PlateTileException : Exception
    {
        public PlateTileException(string message)
            : base(message)
        {
        }

        public PlateTileException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
