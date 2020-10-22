using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using WWT.Providers;

namespace WWTMVC5
{
    internal class SystemWebWwtContext : IWwtContext, IRequest, IResponse, IHeaders, IParameters, ICache
    {
        private readonly HttpContext _context;

        public SystemWebWwtContext(HttpContext context)
        {
            _context = context;
        }

        public string MachineName => _context.Server.MachineName;

        public string MapPath(string path) => _context.Server.MapPath(path);

        public IRequest Request => this;

        public IResponse Response => this;

        public ICache Cache => this;

        object ICache.Get(string key) => _context.Cache.Get(key);

        object ICache.this[string key]
        {
            get => _context.Cache[key];
            set => _context.Cache[key] = value;
        }

        object ICache.Add(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
            => _context.Cache.Add(key, value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, null);

        void ICache.Remove(string key) => _context.Cache.Remove(key);

        string IRequest.GetParams(string name) => _context.Request.Params[name];

        string IHeaders.this[string name] => _context.Request.Headers[name];

        string IParameters.this[string name] => _context.Request.Params[name];

        IParameters IRequest.Params => this;

        IHeaders IRequest.Headers => this;

        Uri IRequest.Url => _context.Request.Url;

        bool IRequest.ContainsCookie(string name) => _context.Request.Cookies[name] != null;

        string IRequest.UserAgent => _context.Request.UserAgent;

        string IRequest.PhysicalPath => _context.Request.PhysicalPath;

        Stream IRequest.InputStream => _context.Request.InputStream;

        string IResponse.ContentType
        {
            get => _context.Response.ContentType;
            set => _context.Response.ContentType = value;
        }

        string IResponse.Status
        {
            get => _context.Response.Status;
            set => _context.Response.Status = value;
        }

        int IResponse.StatusCode
        {
            get => _context.Response.StatusCode;
            set => _context.Response.StatusCode = value;
        }

        Stream IResponse.OutputStream => _context.Response.OutputStream;

        int IResponse.Expires
        {
            get => _context.Response.Expires;
            set => _context.Response.Expires = value;
        }

        string IResponse.CacheControl
        {
            get => _context.Response.CacheControl;
            set => _context.Response.CacheControl = value;
        }
        void IResponse.AddHeader(string name, string value) => _context.Response.AddHeader(name, value);
        void IResponse.BinaryWrite(byte[] data) => _context.Response.BinaryWrite(data);
        void IResponse.Clear() => _context.Response.Clear();
        void IResponse.ClearHeaders() => _context.Response.ClearHeaders();
        void IResponse.Close() => _context.Response.Close();
        void IResponse.End() => _context.Response.End();
        void IResponse.Flush() => _context.Response.Flush();
        void IResponse.Redirect(string redirectUri) => _context.Response.Redirect(redirectUri);
        void IResponse.Write(string message) => _context.Response.Write(message);
        void IResponse.WriteFile(string path) => _context.Response.WriteFile(path);
    }
}
