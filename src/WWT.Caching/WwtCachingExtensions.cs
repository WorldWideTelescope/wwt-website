using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swick.Cache;
using Swick.Cache.Handlers;
using Swick.Cache.Json;
using System;
using System.Reflection;

namespace WWT
{
    public static class WwtCachingExtensions
    {
        public static WwtCacheBuilder AddCaching(this IServiceCollection services, Action<Caching.CachingOptions> configure)
        {
            var options = new Caching.CachingOptions();

            configure(options);

            services.AddSingleton(options);

            if (!options.UseCaching)
            {
                return new WwtCacheBuilder(null);
            }

            var cacheBuilder = services.AddCachingManager()
                .Configure(opts =>
                {
                    opts.CacheHandlers.Add(new WwtCacheHandler(options.SlidingExpiration));
                })
                .AddJsonSerializer(opts =>
                {
                    opts.JsonOptions = new System.Text.Json.JsonSerializerOptions();
                    opts.JsonOptions.Converters.Add(new StreamConverter());
                });

            if (!string.IsNullOrEmpty(options.RedisCacheConnectionString))
            {
                services.AddStackExchangeRedisCache(redisOptions =>
                {
                    redisOptions.Configuration = options.RedisCacheConnectionString;
                });

                services.Decorate<IDistributedCache>((other, ctx) => new AppInsightsDistributedCache(ctx.GetRequiredService<IOptions<TelemetryConfiguration>>(), other));
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            return new WwtCacheBuilder(cacheBuilder);
        }

        public class WwtCacheBuilder
        {
            private readonly CacheBuilder _builder;

            public WwtCacheBuilder(CacheBuilder builder)
            {
                _builder = builder;
            }

            public WwtCacheBuilder CacheType<T>(Action<CacheTypeBuilder<T>> config)
                where T : class
            {
                if (_builder is null)
                {
                    return this;
                }

                _builder.CacheType<T>(config);
                _builder.Services.Decorate<T>((other, ctx) => ctx.GetRequiredService<ICachingManager>().CreateCachedProxy(other));

                return this;
            }
        }

        private class WwtCacheHandler : CacheHandler
        {
            private readonly TimeSpan _slidingExpiration;

            public WwtCacheHandler(TimeSpan slidingExpiration)
            {
                _slidingExpiration = slidingExpiration;
            }

            protected override void ConfigureEntryOptions(Type type, MethodInfo method, object obj, DistributedCacheEntryOptions entry)
            {
                entry.SlidingExpiration = _slidingExpiration;
            }

            /// <summary>
            /// Many of the return types have a Stream in them, which is not round-trippable and must be deserialized again before returning it.
            /// </summary>
            protected override bool IsDataDrained(object obj) => true;
        }
    }
}
