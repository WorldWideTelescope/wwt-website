using System;
using System.IO;
using System.Web.Caching;
using System.Web.UI;

namespace WWT.Providers
{
    internal class PageWwtContext : IWwtContext, IRequest, IResponse, IHeaders, IParameters, ICache
    {
        private readonly Page _page;

        public PageWwtContext(Page page)
        {
            _page = page;
        }

        public ICache Cache => this;

        object ICache.Get(string key) => _page.Cache.Get(key);

        object ICache.this[string key]
        {
            get => _page.Cache[key];
            set => _page.Cache[key] = value;
        }

        object ICache.Add(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
            => _page.Cache.Add(key, value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Normal, null);

        void ICache.Remove(string key) => _page.Cache.Remove(key);

        public string MachineName => _page.Server.MachineName;

        public string MapPath(string path) => _page.Server.MapPath(path);

        public IRequest Request => this;

        public IResponse Response => this;

        string IRequest.GetParams(string name) => _page.Request.Params[name];

        string IHeaders.this[string name] => _page.Request.Headers[name];

        string IParameters.this[string name] => _page.Request.Params[name];

        IParameters IRequest.Params => this;

        IHeaders IRequest.Headers => this;

        Uri IRequest.Url => _page.Request.Url;

        bool IRequest.ContainsCookie(string name) => _page.Request.Cookies[name] != null;

        string IRequest.UserAgent => _page.Request.UserAgent;

        string IRequest.PhysicalPath => _page.Request.PhysicalPath;

        Stream IRequest.InputStream => _page.Request.InputStream;

        string IResponse.ContentType
        {
            get => _page.Response.ContentType;
            set => _page.Response.ContentType = value;
        }

        string IResponse.Status
        {
            get => _page.Response.Status;
            set => _page.Response.Status = value;
        }

        int IResponse.StatusCode
        {
            get => _page.Response.StatusCode;
            set => _page.Response.StatusCode = value;
        }

        Stream IResponse.OutputStream => _page.Response.OutputStream;

        int IResponse.Expires
        {
            get => _page.Response.Expires;
            set => _page.Response.Expires = value;
        }

        string IResponse.CacheControl
        {
            get => _page.Response.CacheControl;
            set => _page.Response.CacheControl = value;
        }
        void IResponse.AddHeader(string name, string value) => _page.Response.AddHeader(name, value);
        void IResponse.BinaryWrite(byte[] data) => _page.Response.BinaryWrite(data);
        void IResponse.Clear() => _page.Response.Clear();
        void IResponse.ClearHeaders() => _page.Response.ClearHeaders();
        void IResponse.Close() => _page.Response.Close();
        void IResponse.End() => _page.Response.End();
        void IResponse.Flush() => _page.Response.Flush();
        void IResponse.Redirect(string redirectUri) => _page.Response.Redirect(redirectUri);
        void IResponse.Write(string message) => _page.Response.Write(message);
        void IResponse.WriteFile(string path) => _page.Response.WriteFile(path);
    }
}
