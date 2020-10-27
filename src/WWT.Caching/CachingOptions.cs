using System;

namespace WWT.Caching
{
    public class CachingOptions
    {
        public bool UseCaching { get; set; }

        public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);

        public string RedisCacheConnectionString { get; set; }
    }
}
