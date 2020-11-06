using System;
using System.Collections.Generic;

namespace WWT.Providers
{
    public class EndpointManager
    {
        private readonly Dictionary<string, Type> _map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public void Add(string endpoint, Type type)
            => _map.Add(endpoint, type);

        public bool TryGetType(string endpoint, out Type type)
            => _map.TryGetValue(endpoint, out type);
    }
}
