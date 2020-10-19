using StackExchange.Redis;
using System;
using WWTWebservices;

namespace WWT.PlateFiles.Caching
{
    public class RedisCachedPlateTilePyramid : CachedPlateTilePyramid
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly TimeSpan _expiry;

        public RedisCachedPlateTilePyramid(
            IPlateTilePyramid other,
            IConnectionMultiplexer connection,
            CachingOptions options)
            : base(other)
        {
            _connection = connection;
            _expiry = options.SlidingExpiration;
        }

        protected override byte[] GetOrUpdateCache(TileContext context)
        {
            var key = context.GetKey();

            var db = _connection.GetDatabase();
            var cached = db.StringGet(key);

            if (cached.HasValue)
            {
                return cached;
            }
            else
            {
                var result = context.GetResult();

                // No need to block for the transfer to actually complete. The caching is done asynchronously and
                // will be available at some point in the near future.
                db.StringSet(key, result, _expiry, When.Always, CommandFlags.FireAndForget);

                return result;
            }
        }
    }
}
