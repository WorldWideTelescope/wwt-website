using System.IO;

namespace WWT.Providers
{
    public interface IResponse
    {
        void BinaryWrite(byte[] data);

        void End();

        void Clear();

        string ContentType { get; set; }

        void AddHeader(string name, string value);

        void WriteFile(string path);

        string Status { get; set; }

        int StatusCode { get; set; }

        void Write(string message);

        void Flush();

        Stream OutputStream { get; }

        void Close();

        void ClearHeaders();

        void Redirect(string redirectUri);

        int Expires { get; set; }

        string CacheControl { get; set; }
    }
}
