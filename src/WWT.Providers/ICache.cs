#nullable disable

using System;

namespace WWT.Providers
{
    public interface ICache
    {
        object this[string key] { get; set; }

        object Get(string key);

        object Add(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration);

        void Remove(string key);
    }
}
