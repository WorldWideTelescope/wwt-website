using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT
{
    public static class StreamExtensions
    {
        /// <summary>
        /// This is available on platforms after .NET Standard 2.0, but this mimics the general shape so we don't have deal with a buffer size.
        /// Per the documentation, the default buffer size is 81920 bytes.
        /// </summary>
        public static Task CopyToAsync(this Stream stream, Stream destination, CancellationToken token)
            => stream.CopyToAsync(destination, 81920, token);
    }
}
