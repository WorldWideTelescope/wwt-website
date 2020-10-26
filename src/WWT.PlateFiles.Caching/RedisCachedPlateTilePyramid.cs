using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using WWTWebservices;

namespace WWT.PlateFiles.Caching
{
    public class RedisCachedPlateTilePyramid : CachedPlateTilePyramid
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly ILogger<RedisCachedPlateTilePyramid> _logger;
        private readonly TimeSpan _expiry;

        public RedisCachedPlateTilePyramid(
            IPlateTilePyramid other,
            IConnectionMultiplexer connection,
            ILogger<RedisCachedPlateTilePyramid> logger,
            CachingOptions options)
            : base(other)
        {
            _connection = connection;
            _logger = logger;
            _expiry = options.SlidingExpiration;
        }

        protected override byte[] GetOrUpdateCache(TileContext context)
        {
            try
            {
                return GetFromCache(context);
            }
            catch (RedisException e)
            {
                _logger.LogError(e, "Error accessing cache. Defaulting to direct Azure access.");
                return context.GetResult();
            }
        }

        private byte[] GetFromCache(TileContext context)
        {
            var key = context.GetKey();

            var db = _connection.GetDatabase();
            var cached = db.StringGet(key);

            if (cached.HasValue)
            {
                _logger.LogInformation("Using cached value for request");
                return cached;
            }
            else
            {
                _logger.LogInformation("No cached value found. Retrieving from Azure and saving in cache");

                var result = context.GetResult();

                // No need to block for the transfer to actually complete. The caching is done asynchronously and
                // will be available at some point in the near future.
                db.StringSet(key, result, _expiry, When.Always, CommandFlags.FireAndForget);

                return result;
            }
        }
    }
}
