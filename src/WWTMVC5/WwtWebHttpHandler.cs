using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using WWT.Providers;

namespace WWTMVC5
{
    public class WwtWebHttpHandler : HttpTaskAsyncHandler
    {
        private static Func<string, ScopedRequest> _factory;

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            using (var scope = _factory(context.Request.Path))
            {
                var logger = scope.Resolve<ILogger<WwtWebHttpHandler>>();

                // AppInsights modules will populate the current activity. This ensures that the subsequent
                // calls to ILogger<> will have the correct settings as well.
                // NOTE: This will be available by default with v5.0.0 of the logging library
                var activity = Activity.Current;
                var scopes = new Dictionary<string, object>
                    {
                        { "ParentId", activity.ParentSpanId.ToHexString() },
                        { "SpanId", activity.SpanId.ToHexString() },
                        { "TraceId", activity.TraceId.ToHexString() },
                        { "TraceFlags", activity.ActivityTraceFlags },
                        { "TraceState", activity.TraceStateString },
                    };

                using (logger.BeginScope(scopes))
                {
                    if (scope.Provider is null)
                    {
                        logger.LogError("No known route for {Path}", context.Request.Path);

                        context.Response.StatusCode = 404;
                    }
                    else
                    {
                        context.Response.ContentType = scope.Provider.ContentType;

                        logger.LogInformation("Dispatch {Path} to {Provider}", context.Request.Path, scope.Provider.GetType());
                        await scope.Provider.RunAsync(new SystemWebWwtContext(context), context.Response.ClientDisconnectedToken);
                    }
                }
            }
        }

        public override bool IsReusable => true;

        public static void Initialize(IServiceProvider provider)
        {
            var endpoints = provider.GetRequiredService<EndpointManager>();

            ScopedRequest GetProvider(string endpoint)
            {
                var scope = provider.CreateScope();

                if (endpoints.TryGetType(endpoint, out var type))
                {
                    return new ScopedRequest(scope, (RequestProvider)scope.ServiceProvider.GetRequiredService(type));
                }
                else
                {
                    return new ScopedRequest(scope, null);
                }
            }

            _factory = GetProvider;
        }

        private class ScopedRequest : IDisposable
        {
            private readonly IServiceScope _scope;

            public ScopedRequest(IServiceScope scope, RequestProvider provider)
            {
                _scope = scope;
                Provider = provider;
            }

            public RequestProvider Provider { get; }

            public T Resolve<T>() => _scope.ServiceProvider.GetRequiredService<T>();

            public void Dispose()
            {
                _scope.Dispose();
            }
        }
    }
}
