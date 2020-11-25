#nullable disable

using System.Collections.Specialized;

namespace WWT.Providers
{
    public interface IParameters
    {
        string this[string p] { get; }
    }
}
