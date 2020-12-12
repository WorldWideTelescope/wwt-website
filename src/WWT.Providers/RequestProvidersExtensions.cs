#nullable disable

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using WWT.Providers.Services;
using WWTWebservices;

namespace WWT.Providers
{
    public static class RequestProvidersExtensions
    {
        public static IServiceCollection AddRequestProviders(this IServiceCollection services, Action<WwtOptions> config)
        {
            var manager = new EndpointManager();
            var types = typeof(RequestProvider).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(RequestProvider).IsAssignableFrom(t));

            foreach (var type in types)
            {
                services.AddSingleton(type);

                foreach (var endpoint in type.GetCustomAttributes<RequestEndpointAttribute>())
                {
                    manager.Add(endpoint.Endpoint, type);
                }
            }

            services.AddSingleton(manager);

            services.AddSingleton<IExternalUrlInfo, WwtExternalUrlInfo>();
            services.AddSingleton<IFileNameHasher, Net4x32BitFileNameHasher>();
            services.AddSingleton<IOctTileMapBuilder, OctTileMapBuilder>();
            services.AddSingleton<IMandelbrot, Mandelbrot>();
            services.AddSingleton<IVirtualEarthDownloader, VirtualEarthDownloader>();
            services.AddSingleton<IDevDataAccessor, DevDataAccessor>();

            var options = new WwtOptions();
            config(options);
            services.AddSingleton(options);

            return services;
        }
    }
}
