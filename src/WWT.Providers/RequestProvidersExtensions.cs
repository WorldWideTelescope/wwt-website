using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WWT.Providers.Services;

namespace WWT.Providers
{
    public static class RequestProvidersExtensions
    {
        public static void AddRequestProviders(this IServiceCollection services, Action<FilePathOptions> config)
        {
            var types = typeof(RequestProvider).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(RequestProvider).IsAssignableFrom(t));

            foreach (var type in types)
            {
                services.AddSingleton(type);
            }

            services.AddSingleton<IMandelbrot, Mandelbrot>();

            var options = new FilePathOptions();

            config(options);

            services.AddSingleton(options);
        }

        /// <summary>
        /// This is available on platforms after .NET Standard 2.0, but this mimics the general shape so we don't have deal with a buffer size.
        /// Per the documentation, the default buffer size is 81920 bytes.
        /// </summary>
        internal static Task CopyToAsync(this Stream stream, Stream destination, CancellationToken token)
            => stream.CopyToAsync(destination, 81920, token);
    }
}
