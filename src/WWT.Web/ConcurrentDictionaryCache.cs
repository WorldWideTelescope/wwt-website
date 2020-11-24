#nullable disable

using System;
using System.Collections.Concurrent;
using WWT.Providers;

namespace WWT.Web
{
    public class ConcurrentDictionaryCache : ICache
    {
        private readonly ConcurrentDictionary<string, object> _cache;

        public ConcurrentDictionaryCache()
        {
            _cache = new ConcurrentDictionary<string, object>();
        }

        public object this[string key]
        {
            get
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    return value;
                }

                return null;
            }
            set => _cache.AddOrUpdate(key, value, (s, o) => o);
        }

        public object Add(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
            => this[key] = value;

        public object Get(string key) => this[key];

        public void Remove(string key) => _cache.TryRemove(key, out _);
    }

}
