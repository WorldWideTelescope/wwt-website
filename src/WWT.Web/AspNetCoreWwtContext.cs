#nullable disable

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
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

        public AspNetCoreWwtContext(HttpContext ctx, ICache cache)
        {
            _ctx = ctx;
            Cache = cache;
        }

        string IParameters.this[string p] => _ctx.Request.Query[p];

        string IHeaders.this[string p] => _ctx.Request.Headers[p];

        public ICache Cache { get; }

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

        IParameters IRequest.Params => this;

        string IRequest.GetParams(string name) => _ctx.Request.Query[name];

        IHeaders IRequest.Headers => this;

        Uri IRequest.Url => new Uri(_ctx.Request.GetEncodedUrl());

        string IRequest.UserAgent => _ctx.Request.Headers["User-Agent"];

        Stream IRequest.InputStream => _ctx.Request.Body;

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

        Task IResponse.ServeStreamAsync(Stream stream, string contentType, string etag)
        {
            var e = _ctx.RequestServices.GetRequiredService<IActionResultExecutor<FileStreamResult>>();
            var route = _ctx.GetRouteData();
            var actionContext = new ActionContext(_ctx, route, new ActionDescriptor());

            var result = new FileStreamResult(stream, contentType) {
                EnableRangeProcessing = true,
                EntityTag = EntityTagHeaderValue.Parse(etag),
            };

            return e.ExecuteAsync(actionContext, result);
        }
    }
}
