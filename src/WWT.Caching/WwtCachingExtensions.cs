using Microsoft.Extensions.DependencyInjection;
using Swick.Cache;
using Swick.Cache.Handlers;
using System;

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
                    opts.CacheHandlers.Add(new DefaultExpirationCacheHandler(entry =>
                    {
                        entry.SlidingExpiration = options.SlidingExpiration;
                    }));
                });

            if (!string.IsNullOrEmpty(options.RedisCacheConnectionString))
            {
                services.AddStackExchangeRedisCache(redisOptions =>
                {
                    redisOptions.Configuration = options.RedisCacheConnectionString;
                });
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
    }
}
