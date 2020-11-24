#nullable disable

using System;

namespace WWT.Providers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequestEndpointAttribute : Attribute
    {
        public RequestEndpointAttribute(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
