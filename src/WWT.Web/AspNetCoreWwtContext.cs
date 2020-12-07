#nullable disable

using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWT.Providers;

namespace WWT.Web
{
    public class AspNetCoreWwtContext : IWwtContext, IRequest, IResponse, IHeaders, IParameters
    {
        private readonly HttpContext _ctx;
        private readonly ICache _cache;

        public AspNetCoreWwtContext(HttpContext ctx, ICache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }

        string IParameters.this[string p] => _ctx.Request.Query[p];

        string IHeaders.this[string p] => _ctx.Request.Headers[p];

        public ICache Cache => _cache;

        public IRequest Request => this;

        public IResponse Response => this;

        public string MachineName => Environment.MachineName;

        string IResponse.ContentType
        {
            get => _ctx.Response.ContentType;
            set => _ctx.Response.ContentType = value;
        }

        int IResponse.StatusCode
        {
            get => _ctx.Response.StatusCode;
            set => _ctx.Response.StatusCode = value;
        }

        Stream IResponse.OutputStream => _ctx.Response.Body;

        int IResponse.Expires
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        string IResponse.CacheControl
        {
            get => throw new NotImplementedException();
            set => _ctx.Response.Headers.Add("Cache-Control", value);
        }

        IParameters IRequest.Params => this;

        string IRequest.GetParams(string name) => _ctx.Request.Query[name];

        IHeaders IRequest.Headers => this;

        Uri IRequest.Url => new Uri($"{_ctx.Request.Scheme}://{_ctx.Request.Host}{_ctx.Request.Path}");

        string IRequest.UserAgent => _ctx.Request.Headers["User-Agent"];

        string IRequest.PhysicalPath => throw new NotImplementedException();

        Stream IRequest.InputStream => _ctx.Request.Body;

        public string MapPath(params string[] path) => throw new NotImplementedException();

        void IResponse.AddHeader(string name, string value) => _ctx.Response.Headers.Add(name, value);

        void IResponse.Clear()
        {
        }

        void IResponse.ClearHeaders()
        {
        }

        void IResponse.Close()
        {
        }

        bool IRequest.ContainsCookie(string name) => _ctx.Request.Cookies.ContainsKey(name);

        void IResponse.End()
        {
        }

        void IResponse.Flush()
        {
        }

        Task IResponse.WriteAsync(string message, CancellationToken token) => _ctx.Response.WriteAsync(message, token);

        void IResponse.Redirect(string redirectUri) => _ctx.Response.Redirect(redirectUri);

        void IResponse.WriteFile(string path) => throw new NotImplementedException();
    }
}
