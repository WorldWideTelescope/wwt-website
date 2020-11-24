#nullable disable

using System;
using System.IO;

namespace WWT.Providers
{
    public interface IRequest
    {
        IParameters Params { get; }

        IHeaders Headers { get; }

        bool ContainsCookie(string name);

        string GetParams(string name);

        Uri Url { get; }

        string UserAgent { get; }

        string PhysicalPath { get; }

        Stream InputStream { get; }
    }
}
