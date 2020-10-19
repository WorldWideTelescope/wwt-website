using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

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

            var options = new FilePathOptions();

            config(options);

            services.AddSingleton(options);
        }
    }
}
