using Microsoft.Extensions.DependencyInjection;
using Swick.Cache;
using Swick.Cache.Handlers;
using System;
using WWTWebservices;

namespace WWT
{
    public static class WwtCachingExtensions
    {
        public static void AddCaching(this IServiceCollection services, Action<Caching.CachingOptions> configure)
        {
            var options = new Caching.CachingOptions();

            configure(options);

            services.AddSingleton(options);

            if (options.UseCaching)
            {
                services.AddCachingManager()
                    .Configure(opts =>
                    {
                        opts.CacheHandlers.Add(new DefaultExpirationCacheHandler(entry =>
                        {
                            entry.SlidingExpiration = options.SlidingExpiration;
                        }));
                    })
                    .CacheType<IPlateTilePyramid>(plates => plates.Add(nameof(IPlateTilePyramid.GetStream)));

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

                services.Decorate<IPlateTilePyramid>((other, ctx) => ctx.GetRequiredService<ICachingManager>().CreateCachedProxy(other));
            }
        }
    }
}
