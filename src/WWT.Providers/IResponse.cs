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

        void WriteFile(string path);

        string Status { get; set; }

        int StatusCode { get; set; }

        Task WriteAsync(string message, CancellationToken token);

        void Flush();

        Stream OutputStream { get; }

        void Close();

        void ClearHeaders();

        void Redirect(string redirectUri);

        int Expires { get; set; }

        string CacheControl { get; set; }
    }
}
