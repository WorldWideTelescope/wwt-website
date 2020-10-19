using Microsoft.Extensions.Caching.Memory;
using WWTWebservices;

namespace WWT.PlateFiles.Caching
{
    public class InMemoryCachedPlateTilePyramid : CachedPlateTilePyramid
    {
        private readonly MemoryCache _cache;
        private readonly MemoryCacheEntryOptions _options;

        public InMemoryCachedPlateTilePyramid(IPlateTilePyramid other, CachingOptions options)
            : base(other)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = options.SlidingExpiration
            };
        }

        protected override byte[] GetOrUpdateCache(TileContext context)
        {
            var key = context.GetKey();

            if (_cache.TryGetValue(key, out var cached) && cached is byte[] cachedBytes)
            {
                return cachedBytes;
            }

            var result = context.GetResult();

            _cache.Set(key, result, _options);

            return result;
        }
    }
}
