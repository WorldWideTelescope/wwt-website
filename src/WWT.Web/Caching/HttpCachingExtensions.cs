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
    public static IHttpClientBuilder AddRequestCaching(this IHttpClientBuilder builder, Action<CachingOptions> configure)
    {
        builder.Services.AddSingleton<CachingHandler>();
        builder.AddHttpMessageHandler<CachingHandler>();

        builder.Services.AddOptions<CachingOptions>()
            .Configure(configure);

        return builder;
    }

    public sealed class CachingOptions
    {
        public bool IsEnabled { get; set; }

        public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);

        public IDictionary<string, CachedHostOptions> Hosts { get; } = new Dictionary<string, CachedHostOptions>(StringComparer.OrdinalIgnoreCase);
    }

    public sealed class CachedHostOptions
    {
    }

    private sealed partial class CachingHandler(IOptions<CachingOptions> options, IBufferDistributedCache cache, RecyclableMemoryStreamManager manager) : DelegatingHandler
    {
        private readonly CachingOptions _options = options.Value;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_options.IsEnabled && request.Method == HttpMethod.Get && _options.Hosts.ContainsKey(request.RequestUri?.Host!))
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
