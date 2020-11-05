using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using WWT.Providers;

namespace WWTMVC5
{
    public class WwtWebHttpHandler : HttpTaskAsyncHandler
    {
        private static EndpointManager _endpoints;

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            using (var scope = _endpoints.GetRequestScope(context.Request.Path))
            {
                var logger = scope.Resolve<ILogger<WwtWebHttpHandler>>();

                // AppInsights modules will populate the current activity. This ensures that the subsequent
                // calls to ILogger<> will have the correct settings as well.
                // NOTE: This will be available by default with v5.0.0 of the logging library
                var activity = Activity.Current;
                var scopes = new Dictionary<string, object>
                    {
                        { "ParentId", activity.ParentSpanId.ToHexString() },
                        { "SpanId", activity.SpanId.ToHexString() },
                        { "TraceId", activity.TraceId.ToHexString() },
                        { "TraceFlags", activity.ActivityTraceFlags },
                        { "TraceState", activity.TraceStateString },
                    };

                using (logger.BeginScope(scopes))
                {
                    if (scope.Provider is null)
                    {
                        logger.LogError("No known route for {Path}", context.Request.Path);

                        context.Response.StatusCode = 404;
                    }
                    else
                    {
                        context.Response.ContentType = scope.Provider.ContentType;

                        logger.LogInformation("Dispatch {Path} to {Provider}", context.Request.Path, scope.Provider.GetType());
                        await scope.Provider.RunAsync(new SystemWebWwtContext(context), context.Response.ClientDisconnectedToken);
                    }
                }
            }
        }

        public override bool IsReusable => true;

        public static void Initialize(IServiceProvider provider)
        {
            var endpoints = new EndpointManager(provider);

            endpoints.Add<TwoMASSOctProvider>("/wwtweb/2MASSOct.aspx");
            endpoints.Add<BingDemTileProvider>("/wwtweb/BingDemTile.aspx");
            endpoints.Add<BingDemTile2Provider>("/wwtweb/BingDemTile2.aspx");
            endpoints.Add<CatalogProvider>("/wwtweb/Catalog.aspx");
            endpoints.Add<Catalog2Provider>("/wwtweb/Catalog2.aspx");
            endpoints.Add<DemProvider>("/wwtweb/dem.aspx");
            endpoints.Add<DembathProvider>("/wwtweb/dembath.aspx");
            endpoints.Add<DemMarsProvider>("/wwtweb/demmars.aspx");
            endpoints.Add<DemMarsNewProvider>("/wwtweb/demmars_new.aspx");
            endpoints.Add<DemTileProvider>("/wwtweb/demTile.aspx");
            endpoints.Add<DSSProvider>("/wwtweb/DSS.aspx");
            endpoints.Add<DSSToastProvider>("/wwtweb/DSSToast.aspx");
            endpoints.Add<DustToastProvider>("/wwtweb/DustToast.aspx");
            endpoints.Add<EarthBlendProvider>("/wwtweb/EarthBlend.aspx");
            endpoints.Add<EarthMerBathProvider>("/wwtweb/EarthMerBath.aspx");
            endpoints.Add<ExcelAddinUpdateProvider>("/wwtweb/ExcelAddinUpdate.aspx");
            endpoints.Add<FixedAltitudeDemTileProvider>("/wwtweb/FixedAltitudeDemTile.aspx");
            endpoints.Add<G360Provider>("/wwtweb/g360.aspx");
            endpoints.Add<Galex4FarProvider>("/wwtweb/Galex4Far.aspx");
            endpoints.Add<Galex4NearProvider>("/wwtweb/Galex4Near.aspx");
            endpoints.Add<GalexToastProvider>("/wwtweb/GalexToast.aspx");
            endpoints.Add<GetAuthorThumbnailProvider>("/wwtweb/GetAuthorThumbnail.aspx");
            endpoints.Add<GetHostNameProvider>("/wwtweb/GetHostName.aspx");
            endpoints.Add<GetTileProvider>("/wwtweb/GetTile.aspx");
            endpoints.Add<GetTourProvider>("/wwtweb/GetTour.aspx");
            endpoints.Add<GetTourFileProvider>("/wwtweb/GetTourFile.aspx");
            endpoints.Add<GetTourFileProvider>("/GetTourFile.aspx");
            endpoints.Add<GetTourFileProvider>("/GetTourFile2.aspx");
            endpoints.Add<GetTourListProvider>("/wwtweb/GetTourList.aspx");
            endpoints.Add<GetToursProvider>("/wwtweb/GetTours.aspx");
            endpoints.Add<GetTourThumbnailProvider>("/wwtweb/GetTourThumbnail.aspx");
            endpoints.Add<GlimpseProvider>("/wwtweb/Glimpse.aspx");
            endpoints.Add<GotoProvider>("/wwtweb/Goto.aspx");
            endpoints.Add<Goto2Provider>("/wwtweb/Goto2.aspx");
            endpoints.Add<HAlphaToastProvider>("/wwtweb/hAlphaToast.aspx");
            endpoints.Add<HiriseProvider>("/wwtweb/Hirise.aspx");
            endpoints.Add<HiriseDemProvider>("/wwtweb/HiriseDem.aspx");
            endpoints.Add<HiriseDem2Provider>("/wwtweb/HiriseDem2.aspx");
            endpoints.Add<HiriseDem3Provider>("/wwtweb/HiriseDem3.aspx");
            endpoints.Add<IsstleProvider>("/wwtweb/isstle.aspx");
            endpoints.Add<Isstle2Provider>("/wwtweb/isstle2.aspx");
            endpoints.Add<JupiterProvider>("/wwtweb/jupiter.aspx");
            endpoints.Add<LoginProvider>("/wwtweb/login.aspx");
            endpoints.Add<MandelProvider>("/wwtweb/Mandel.aspx");
            endpoints.Add<Mandel1Provider>("/wwtweb/mandel1.aspx");
            endpoints.Add<MarsProvider>("/wwtweb/mars.aspx");
            endpoints.Add<MarsdemProvider>("/wwtweb/marsdem.aspx");
            endpoints.Add<MarsHiriseProvider>("/wwtweb/MarsHirise.aspx");
            endpoints.Add<MarsMocProvider>("/wwtweb/MarsMoc.aspx");
            endpoints.Add<MartianTileProvider>("/wwtweb/MartianTile.aspx");
            endpoints.Add<MartianTile2Provider>("/wwtweb/MartianTile2.aspx");
            endpoints.Add<MartianTileProvider>("/wwtweb/MartianTileNew.aspx");
            endpoints.Add<MipsgalProvider>("/wwtweb/Mipsgal.aspx");
            endpoints.Add<MoondemProvider>("/wwtweb/moondem.aspx");
            endpoints.Add<MoonOctProvider>("/wwtweb/moonOct.aspx");
            endpoints.Add<MoontoastProvider>("/wwtweb/moontoast.aspx");
            endpoints.Add<MoontoastdemProvider>("/wwtweb/moontoastdem.aspx");
            endpoints.Add<PostMarsProvider>("/wwtweb/postmars.aspx");
            endpoints.Add<PostMarsProvider>("/wwtweb/postmarsdem.aspx");
            endpoints.Add<PostMarsProvider>("/wwtweb/postmarsdem2.aspx");
            endpoints.Add<PostRatingFeedbackProvider>("/wwtweb/PostRatingFeedback.aspx");
            endpoints.Add<RassToastProvider>("/wwtweb/RassToast.aspx");
            endpoints.Add<SDSS12ToastProvider>("/wwtweb/SDSS12Toast.aspx");
            endpoints.Add<SDSSToastProvider>("/wwtweb/SDSSToast.aspx");
            endpoints.Add<SDSSToastOfflineProvider>("/wwtweb/SDSSToast.offline.aspx");
            endpoints.Add<SDSSToast2Provider>("/wwtweb/SDSSToast2.aspx");
            endpoints.Add<ShowImageProvider>("/wwtweb/ShowImage.aspx");
            endpoints.Add<ShowImage2Provider>("/wwtweb/ShowImage2.aspx");
            endpoints.Add<StarChartProvider>("/wwtweb/StarChart.aspx");
            endpoints.Add<TestProvider>("/wwtweb/test.aspx");
            endpoints.Add<TestfailoverProvider>("/wwtweb/testfailover.aspx");
            endpoints.Add<ThumbnailProvider>("/wwtweb/thumbnail.aspx");
            endpoints.Add<TileImageProvider>("/wwtweb/TileImage.aspx");
            endpoints.Add<TilesProvider>("/wwtweb/tiles.aspx");
            endpoints.Add<Tiles2Provider>("/wwtweb/tiles2.aspx");
            endpoints.Add<TilethumbProvider>("/wwtweb/tilethumb.aspx");
            endpoints.Add<TwoMassToastProvider>("/wwtweb/TwoMassToast.aspx");
            endpoints.Add<TychoOctProvider>("/wwtweb/TychoOct.aspx");
            endpoints.Add<VeblendProvider>("/wwtweb/veblend.aspx");
            endpoints.Add<VersionProvider>("/wwtweb/version.aspx");
            endpoints.Add<VersionsProvider>("/wwtweb/versions.aspx");
            endpoints.Add<VlssToastProvider>("/wwtweb/vlssToast.aspx");
            endpoints.Add<WebloginProvider>("/wwtweb/weblogin.aspx");
            endpoints.Add<WebServiceProxyProvider>("/wwtweb/WebServiceProxy.aspx");
            endpoints.Add<WmapProvider>("/wwtweb/wmap.aspx");
            endpoints.Add<XML2WTTProvider>("/wwtweb/XML2WTT.aspx");

            _endpoints = endpoints;
        }

        private class EndpointManager
        {
            private readonly IServiceProvider _services;
            private readonly Dictionary<string, Type> _map;

            public EndpointManager(IServiceProvider provider)
            {
                _services = provider;
                _map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            }

            public void Add<T>(string endpoint)
                where T : RequestProvider
            {
                _map.Add(endpoint, (typeof(T)));
            }

            public ScopedRequest GetRequestScope(string endpoint)
            {
                var scope = _services.CreateScope();

                if (_map.TryGetValue(endpoint, out var result))
                {
                    var provider = (RequestProvider)scope.ServiceProvider.GetRequiredService(result);

                    return new ScopedRequest(scope, provider);
                }
                else
                {
                    return new ScopedRequest(scope, null);
                }
            }
        }

        private class ScopedRequest : IDisposable
        {
            private readonly IServiceScope _scope;

            public ScopedRequest(IServiceScope scope, RequestProvider provider)
            {
                _scope = scope;
                Provider = provider;
            }

            public RequestProvider Provider { get; }

            public T Resolve<T>() => _scope.ServiceProvider.GetRequiredService<T>();

            public void Dispose()
            {
                _scope.Dispose();
            }
        }
    }
}
