using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WWT.Providers.Services;

namespace WWT.Providers
{
    public static class RequestProvidersExtensions
    {
        public static void AddRequestProviders(this IServiceCollection services, Action<WwtOptions> config)
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
            services.AddSingleton<IOctTileMapBuilder, OctTileMapBuilder>();
            services.AddSingleton<IMandelbrot, Mandelbrot>();
            services.AddSingleton<IVirtualEarthDownloader, VirtualEarthDownloader>();

            var options = new WwtOptions();

            config(options);

            services.AddSingleton(options);
        }

        /// <summary>
        /// This is available on platforms after .NET Standard 2.0, but this mimics the general shape so we don't have deal with a buffer size.
        /// Per the documentation, the default buffer size is 81920 bytes.
        /// </summary>
        internal static Task CopyToAsync(this Stream stream, Stream destination, CancellationToken token)
            => stream.CopyToAsync(destination, 81920, token);

        internal static Stream SaveToStream(this Bitmap bitmap, ImageFormat format)
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, format);
            ms.Position = 0;
            return ms;
        }
    }
}
