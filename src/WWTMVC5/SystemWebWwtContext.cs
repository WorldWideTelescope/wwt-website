using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        Stream IRequest.InputStream => _context.Request.InputStream;

        string IResponse.ContentType
        {
            get => _context.Response.ContentType;
            set => _context.Response.ContentType = value;
        }

        int IResponse.StatusCode
        {
            get => _context.Response.StatusCode;
            set => _context.Response.StatusCode = value;
        }

        Stream IResponse.OutputStream => _context.Response.OutputStream;

        void IResponse.AddHeader(string name, string value) => _context.Response.AddHeader(name, value);
        void IResponse.Clear() => _context.Response.Clear();
        void IResponse.ClearHeaders() => _context.Response.ClearHeaders();
        void IResponse.Close() => _context.Response.Close();
        void IResponse.End() => _context.Response.End();
        void IResponse.Flush() => _context.Response.Flush();
        void IResponse.Redirect(string redirectUri) => _context.Response.Redirect(redirectUri);

        Task IResponse.WriteAsync(string message, CancellationToken token)
        {
            _context.Response.Write(message);
            return Task.CompletedTask;
        }

        Task IResponse.ServeStreamAsync(Stream stream, string contentType, string etag)
        {
            // No known reason that we can't implement this; but this API is
            // mainly intended for the data services, not the WWTMVC5 app, and
            // right now I'm not in a position to easily build and test an
            // implementation here.
            throw new NotImplementedException();
        }
    }
}
