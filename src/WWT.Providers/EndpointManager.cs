#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;

namespace WWT.Providers
{
    public class EndpointManager : IEnumerable<(string endpoint, Type requestProviderType)>
    {
        private readonly Dictionary<string, Type> _map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public void Add(string endpoint, Type type)
            => _map.Add(endpoint, type);

        public IEnumerator<(string, Type)> GetEnumerator()
        {
            foreach (var item in _map)
            {
                yield return (item.Key, item.Value);
            }
        }

        public bool TryGetType(string endpoint, out Type type)
            => _map.TryGetValue(endpoint, out type);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
