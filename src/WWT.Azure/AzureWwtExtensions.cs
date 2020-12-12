#nullable disable

using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

using WWT.Catalog;
using WWT.Imaging;
using WWT.PlateFiles;
using WWT.Tours;

namespace WWT.Azure
{
    public static class AzureWwtExtensions
    {
        public static AzureServiceBuilder AddAzureServices(this IServiceCollection services, Action<AzureOptions> configure)
        {
            var options = new AzureOptions();
            configure(options);

            // This may be set earlier on in the host, so we don't want to override it
            services.TryAddSingleton<TokenCredential, DefaultAzureCredential>();
            services.AddSingleton(options);
            services.AddSingleton<AzureServiceAccessor>();
            services.AddSingleton(ctx => ctx.GetRequiredService<AzureServiceAccessor>().WwtFiles);

            return new AzureServiceBuilder(services);
        }

        public static AzureServiceBuilder AddThumbnails(this AzureServiceBuilder services, Action<ThumbnailOptions> configure)
        {
            var thumbnailOptions = new ThumbnailOptions();
            configure(thumbnailOptions);

            services.Services.AddSingleton(thumbnailOptions);
            services.Services.AddSingleton<IThumbnailAccessor, AzureThumbnailAccessor>();

            return services;
        }

        public static AzureServiceBuilder AddCatalog(this AzureServiceBuilder services, Action<AzureCatalogOptions> configure)
        {
            var options = new AzureCatalogOptions();
            configure(options);

            services.Services.AddSingleton(options);
            services.Services.AddSingleton<ICatalogAccessor, AzureCatalogAccessor>();

            return services;
        }

        public static AzureServiceBuilder AddPlateFiles(this AzureServiceBuilder services, Action<AzurePlateTilePyramidOptions> configure)
        {
            var options = new AzurePlateTilePyramidOptions();
            configure(options);

            services.Services.AddSingleton(options);
            services.Services.AddSingleton<IPlateTilePyramid, MarsMolaAwareSeekableAzurePlateTilePyramid>();
            services.Services.AddSingleton<IKnownPlateFiles, AzureKnownPlateFile>();
            services.Services.AddSingleton<IPlateTileDownloader, AzurePlateFileDownloader>();

            return services;
        }

        public static AzureServiceBuilder AddTours(this AzureServiceBuilder services, Action<AzureTourOptions> configure)
        {
            var options = new AzureTourOptions();
            configure(options);

            services.Services.AddSingleton(options);
            services.Services.AddSingleton<ITourAccessor, AzureTourAccessor>();

            return services;
        }

        public static AzureServiceBuilder AddTiles(this AzureServiceBuilder services, Action<AzureTileOptions> configure)
        {
            var options = new AzureTileOptions();
            configure(options);

            services.Services.AddSingleton(options);
            services.Services.AddSingleton<ITileAccessor, AzureTileAccessor>();

            return services;
        }

        public class AzureServiceBuilder
        {
            public AzureServiceBuilder(IServiceCollection services)
            {
                Services = services;
            }

            public IServiceCollection Services { get; }
        }
    }
}
