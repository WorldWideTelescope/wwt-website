using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using WWT.PlateFiles.Caching;
using WWTWebservices;

namespace WWT
{
    public static class WwtCachingExtensions
    {
        public static void AddCaching(this IServiceCollection services, Action<CachingOptions> configure)
        {
            var options = new CachingOptions();

            configure(options);

            services.AddSingleton(options);

            if (options.UseCaching)
            {
                if (!string.IsNullOrEmpty(options.RedisCacheConnectionString))
                {
                    services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options.RedisCacheConnectionString));
                    services.Decorate<IPlateTilePyramid, RedisCachedPlateTilePyramid>();
                }
                else
                {
                    services.Decorate<IPlateTilePyramid, InMemoryCachedPlateTilePyramid>();
                }
            }
        }
    }
}
