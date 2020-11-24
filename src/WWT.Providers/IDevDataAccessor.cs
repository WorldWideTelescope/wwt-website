#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public interface IDevDataAccessor
    {
        Task<Stream> GetDevDataAsync(int maxLevel, CancellationToken token);
    }
}
