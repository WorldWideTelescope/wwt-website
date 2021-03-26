#nullable disable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public interface IResponse
    {
        void End();

        void Clear();

        string ContentType { get; set; }

        void AddHeader(string name, string value);

        int StatusCode { get; set; }

        Task WriteAsync(string message, CancellationToken token);

        void Flush();

        Stream OutputStream { get; }

        Task ServeStreamAsync(Stream stream, string contentType, string etag);

        void Close();

        void ClearHeaders();

        void Redirect(string redirectUri);
    }
}
