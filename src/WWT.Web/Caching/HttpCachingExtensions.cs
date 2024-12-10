using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace WWT.Web.Caching;

internal static class HttpCachingExtensions
{
    public static IHttpClientBuilder AddRequestCaching(this IHttpClientBuilder builder, IEnumerable<string> hosts)
    {
        builder.Services.AddSingleton<CachingHandler>();
        builder.AddHttpMessageHandler<CachingHandler>();

        builder.Services.AddOptions<CachingOptions>()
            .Configure(options => options.Hosts.UnionWith(hosts));

        return builder;
    }

    private sealed class CachingOptions
    {
        public HashSet<string> Hosts { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    private sealed partial class CachingHandler(IOptions<CachingOptions> options, IBufferDistributedCache cache, RecyclableMemoryStreamManager manager) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get && options.Value.Hosts.Contains(request.RequestUri?.Host!))
            {
                return SendCachedAsync(request, cancellationToken);
            }

            return base.SendAsync(request, cancellationToken);
        }

        private RecyclableMemoryStream GetStream() => manager.GetStream("http_cache");

        private async Task<HttpResponseMessage> SendCachedAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var key = $"__http_cache__{request.RequestUri}";

            if (await GetCachedAsync(key, cancellationToken) is { } cached)
            {
                return cached;
            }

            var result = await base.SendAsync(request, cancellationToken);

            if (!result.IsSuccessStatusCode)
            {
                return result;
            }

            var buffer = GetStream();
            var serializer = new ResponseCacheStreamSerializer(buffer);

            result = await serializer.SerializeAsync(result, cancellationToken);

            await cache.SetAsync(key, buffer.GetReadOnlySequence(), new(), cancellationToken);

            return result;
        }

        private async ValueTask<HttpResponseMessage?> GetCachedAsync(string key, CancellationToken cancellationToken)
        {
            var buffer = GetStream();

            if (await cache.TryGetAsync(key, buffer, cancellationToken))
            {
                var serializer = new ResponseCacheStreamSerializer(buffer);

                if (await serializer.DeserializeAsync(cancellationToken) is { } result)
                {
                    return result;
                }
                else
                {
                    // There was a problem with the cached item, so let's remove it
                    await cache.RemoveAsync(key, cancellationToken);
                }
            }

            await buffer.DisposeAsync();
            return null;
        }
    }
}
