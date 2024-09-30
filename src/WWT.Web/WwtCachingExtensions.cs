using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WWT.Imaging;
using WWT.PlateFiles;
using WWT.Providers;
using WWT.Tours;

namespace WWT.Web;

internal static class WwtCachingExtensions
{
    private const string Original = "Original";

    public static void AddWwtCaching(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton(sp => sp.GetRequiredService<ObjectPoolProvider>().CreateStringBuilderPool());
        builder.Services.AddOptions<CachingOptions>()
            .Configure(builder.Configuration.Bind);

        builder.Services.Cache<ITourAccessor, CachedServices>();
        builder.Services.Cache<IThumbnailAccessor, CachedServices>();
        builder.Services.Cache<IPlateTilePyramid, CachedServices>();
        builder.Services.Cache<IOctTileMapBuilder, CachedServices>();
        builder.Services.Cache<IVirtualEarthDownloader, CachedServices>();
        builder.Services.Cache<IMandelbrot, CachedServices>();
    }

    private static void Cache<TService, TImplementation>(this IServiceCollection services)
    {
        var existing = services.Single(f => f.ServiceType == typeof(TService));
        services.Remove(existing);
        services.Add(ServiceDescriptor.DescribeKeyed(existing.ServiceType, Original, existing.ImplementationType, existing.Lifetime));
        services.Add(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), existing.Lifetime));
    }

    private class CachingOptions
    {
        public bool UseCaching { get; set; }

        public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);
    }

    class CachedServices(
        ObjectPool<StringBuilder> sbPool,
        IOptions<CachingOptions> options,
        IDistributedCache cache,
        [FromKeyedServices("WWT")] ActivitySource activitySource,
        [FromKeyedServices(Original)] ITourAccessor tourAccessor,
        [FromKeyedServices(Original)] IThumbnailAccessor thumbnailAccessor,
        [FromKeyedServices(Original)] IPlateTilePyramid plateTilePyramid,
        [FromKeyedServices(Original)] IOctTileMapBuilder octTileMapBuilder,
        [FromKeyedServices(Original)] IVirtualEarthDownloader virtualEarthDownloader,
        [FromKeyedServices(Original)] IMandelbrot mandelbrot
        )
        : ITourAccessor, IThumbnailAccessor, IPlateTilePyramid, IOctTileMapBuilder, IVirtualEarthDownloader, IMandelbrot
    {
        Task<Stream> ITourAccessor.GetAuthorThumbnailAsync(string id, CancellationToken token)
            => GetOrUpdate<ITourAccessor>(() => tourAccessor.GetAuthorThumbnailAsync(id, token), [id], token);

        Task<Stream> ITourAccessor.GetTourAsync(string id, CancellationToken token)
            => GetOrUpdate<ITourAccessor>(() => tourAccessor.GetTourAsync(id, token), [id], token);

        Task<Stream> ITourAccessor.GetTourThumbnailAsync(string id, CancellationToken token)
            => GetOrUpdate<ITourAccessor>(() => tourAccessor.GetTourThumbnailAsync(id, token), [id], token);

        Task<Stream> IThumbnailAccessor.GetThumbnailStreamAsync(string name, string type, CancellationToken token)
            => GetOrUpdate<IThumbnailAccessor>(() => thumbnailAccessor.GetThumbnailStreamAsync(name, type, token), [name, type], token);

        Task<Stream> IThumbnailAccessor.GetDefaultThumbnailStreamAsync(CancellationToken token)
            => GetOrUpdate<IThumbnailAccessor>(() => thumbnailAccessor.GetDefaultThumbnailStreamAsync(token), [], token);

        Task<Stream> IPlateTilePyramid.GetStreamAsync(string pathPrefix, string plateName, int level, int x, int y, CancellationToken token)
            => GetOrUpdate<IPlateTilePyramid>(() => plateTilePyramid.GetStreamAsync(pathPrefix, plateName, level, x, y, token), [pathPrefix, plateName, level, x, y], token);

        Task<Stream> IPlateTilePyramid.GetStreamAsync(string pathPrefix, string plateName, int tag, int level, int x, int y, CancellationToken token)
            => GetOrUpdate<IPlateTilePyramid>(() => plateTilePyramid.GetStreamAsync(pathPrefix, plateName, tag, level, x, y, token), [pathPrefix, plateName, tag, level, x, y], token);

        IAsyncEnumerable<string> IPlateTilePyramid.GetPlateNames(CancellationToken token)
            => plateTilePyramid.GetPlateNames(token);

        Task<Stream> IOctTileMapBuilder.GetOctTileAsync(int level, int tileX, int tileY, bool enforceBoundary, CancellationToken token)
           => GetOrUpdate<IOctTileMapBuilder>(() => octTileMapBuilder.GetOctTileAsync(level, tileX, tileY, enforceBoundary, token), [level, tileX, tileY, enforceBoundary], token);

        int IVirtualEarthDownloader.GetServerID(int x, int y)
           => virtualEarthDownloader.GetServerID(x, y);

        Task<Stream> IVirtualEarthDownloader.DownloadVeTileAsync(VirtualEarthTile tileType, int level, int tileX, int tileY, CancellationToken token)
            => GetOrUpdate<IVirtualEarthDownloader>(() => virtualEarthDownloader.DownloadVeTileAsync(tileType, level, tileX, tileY, token), [tileType, level, tileX, tileY], token);

        int IVirtualEarthDownloader.GetTileAddressFromVEKey(string veKey, out int x, out int y)
            => virtualEarthDownloader.GetTileAddressFromVEKey(veKey, out x, out y);

        string IVirtualEarthDownloader.GetTileID(int x, int y, int level, bool GoogleStyle)
            => virtualEarthDownloader.GetTileID(x, y, level, GoogleStyle);

        protected async Task<Stream> GetOrUpdate<T>(Func<Task<Stream>> run, object[] names, CancellationToken token, [CallerMemberName] string caller = null!)
        {
            using var fullActivity = activitySource.StartActivity($"{typeof(T).FullName}.{caller}");

            if (options.Value.UseCaching)
            {
                fullActivity?.AddTag("UseCaching", true);
                return await GetOrUpdateCache<T>(run, names, token, caller);
            }
            else
            {
                fullActivity?.AddTag("UseCaching", false);
                return await run();
            }
        }

        private async Task<Stream> GetOrUpdateCache<T>(Func<Task<Stream>> run, object[] names, CancellationToken token, [CallerMemberName] string caller = null!)
        {
            var key = GetKey<T>(caller, names);

            using var checkCache = activitySource.StartActivity("check cache");

            if (await cache.GetAsync(key, token) is { } known)
            {
                checkCache?.AddTag("IsCached", true);
                return new MemoryStream(known);
            }

            checkCache?.AddTag("IsCached", false);
            checkCache?.Stop();

            using var runActivity = activitySource.StartActivity("run");
            var created = await run();

            if (created is null)
            {
                runActivity?.SetTag("Size", 0);
                return null;
            }

            var ms = new MemoryStream();
            await created.CopyToAsync(ms, token);

            ms.Position = 0;
            runActivity?.SetTag("Size", ms.Length);
            runActivity?.Stop();

            using var setCache = activitySource.StartActivity("set cache");
            await cache.SetAsync(key, ms.ToArray(), new DistributedCacheEntryOptions { SlidingExpiration = options.Value.SlidingExpiration }, token);

            return ms;
        }

        private string GetKey<T>(string caller, object[] names)
        {
            var sb = sbPool.Get();
            try
            {
                sb.Append(typeof(T).FullName);
                sb.Append('.');
                sb.Append(caller);
                sb.Append('(');

                foreach (var name in names)
                {
                    sb.Append(',');
                    sb.Append(name);
                }

                sb.Append(')');

                return sb.ToString();
            }
            finally
            {
                sbPool.Return(sb);
            }
        }

        Stream IMandelbrot.CreateMandelbrot(int level, int tileX, int tileY)
        {
            using var activity = activitySource.StartActivity("IMandelbrot.CreateMandelbrot");

            return mandelbrot.CreateMandelbrot(level, tileX, tileY);
        }
    }
}