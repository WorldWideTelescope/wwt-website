using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                if (scope is null)
                {
                    context.Response.StatusCode = 404;
                }
                else
                {
                    if (!string.IsNullOrEmpty(scope.ContentType))
                    {
                        context.Response.ContentType = scope.ContentType;
                    }

                    scope.Resolve<ILogger<WwtWebHttpHandler>>().LogInformation("Dispatch {Path} to {Provider}", context.Request.Path, scope.Provider.GetType());

                    await scope.Provider.RunAsync(new SystemWebWwtContext(context), context.Response.ClientDisconnectedToken);
                }
            }
        }

        public override bool IsReusable => true;

        public static void Initialize(IServiceProvider provider)
        {
            var endpoints = new EndpointManager(provider);

            endpoints.Add<TwoMASSOctProvider>("/wwtweb/2MASSOct.aspx", "image/png");
            endpoints.Add<BingDemTileProvider>("/wwtweb/BingDemTile.aspx", "application/octet-stream");
            endpoints.Add<BingDemTile2Provider>("/wwtweb/BingDemTile2.aspx", "application/octet-stream");
            endpoints.Add<CatalogProvider>("/wwtweb/Catalog.aspx", "text/plain");
            endpoints.Add<Catalog2Provider>("/wwtweb/Catalog2.aspx", "text/plain");
            endpoints.Add<DemProvider>("/wwtweb/dem.aspx", "application/octet-stream");
            endpoints.Add<DembathProvider>("/wwtweb/dembath.aspx", "text/plain");
            endpoints.Add<DemMarsProvider>("/wwtweb/demmars.aspx", "application/octet-stream");
            endpoints.Add<DemMarsNewProvider>("/wwtweb/demmars_new.aspx", "application/octet-stream");
            endpoints.Add<DemTileProvider>("/wwtweb/demTile.aspx", "application/octet-stream");
            endpoints.Add<DSSProvider>("/wwtweb/DSS.aspx", "image/png");
            endpoints.Add<DSSToastProvider>("/wwtweb/DSSToast.aspx", "image/png");
            endpoints.Add<DustToastProvider>("/wwtweb/DustToast.aspx", "image/png");
            endpoints.Add<EarthBlendProvider>("/wwtweb/EarthBlend.aspx", "image/png");
            endpoints.Add<EarthMerBathProvider>("/wwtweb/EarthMerBath.aspx", "image/png");
            endpoints.Add<ExcelAddinUpdateProvider>("/wwtweb/ExcelAddinUpdate.aspx", "text/plain");
            endpoints.Add<FixedAltitudeDemTileProvider>("/wwtweb/FixedAltitudeDemTile.aspx", "application/octet-stream");
            endpoints.Add<G360Provider>("/wwtweb/g360.aspx", "image/png");
            endpoints.Add<Galex4FarProvider>("/wwtweb/Galex4Far.aspx", "image/png");
            endpoints.Add<Galex4NearProvider>("/wwtweb/Galex4Near.aspx", "image/png");
            endpoints.Add<GalexToastProvider>("/wwtweb/GalexToast.aspx", "image/png");
            endpoints.Add<GetAuthorThumbnailProvider>("/wwtweb/GetAuthorThumbnail.aspx", "image/png");
            endpoints.Add<GetHostNameProvider>("/wwtweb/GetHostName.aspx", "text/plain");
            endpoints.Add<GetTileProvider>("/wwtweb/GetTile.aspx", "image/png");
            endpoints.Add<GetTourProvider>("/wwtweb/GetTour.aspx", "application/x-wtt");
            endpoints.Add<GetTourFileProvider>("/wwtweb/GetTourFile.aspx", "image/png");
            endpoints.Add<GetTourFileProvider>("/GetTourFile.aspx", "image/png");
            endpoints.Add<GetTourFileProvider>("/GetTourFile2.aspx", "image/png");
            endpoints.Add<GetTourListProvider>("/wwtweb/GetTourList.aspx");
            endpoints.Add<GetToursProvider>("/wwtweb/GetTours.aspx");
            endpoints.Add<GetTourThumbnailProvider>("/wwtweb/GetTourThumbnail.aspx", "image/png");
            endpoints.Add<GlimpseProvider>("/wwtweb/Glimpse.aspx", "image/png");
            endpoints.Add<GotoProvider>("/wwtweb/Goto.aspx", "application/x-wtml");
            endpoints.Add<Goto2Provider>("/wwtweb/Goto2.aspx", "application/x-wtml");
            endpoints.Add<HAlphaToastProvider>("/wwtweb/hAlphaToast.aspx", "image/png");
            endpoints.Add<HiriseProvider>("/wwtweb/Hirise.aspx", "image/png");
            endpoints.Add<HiriseDemProvider>("/wwtweb/HiriseDem.aspx", "text/plain");
            endpoints.Add<HiriseDem2Provider>("/wwtweb/HiriseDem2.aspx", "text/plain");
            endpoints.Add<HiriseDem3Provider>("/wwtweb/HiriseDem3.aspx", "text/plain");
            endpoints.Add<IsstleProvider>("/wwtweb/isstle.aspx", "text/plain");
            endpoints.Add<Isstle2Provider>("/wwtweb/isstle2.aspx", "text/plain");
            endpoints.Add<JupiterProvider>("/wwtweb/jupiter.aspx", "image/png");
            endpoints.Add<LoginProvider>("/wwtweb/login.aspx", "text/plain");
            endpoints.Add<MandelProvider>("/wwtweb/Mandel.aspx", "image/jpeg");
            endpoints.Add<Mandel1Provider>("/wwtweb/mandel1.aspx", "image/jpeg");
            endpoints.Add<MarsProvider>("/wwtweb/mars.aspx", "image/png");
            endpoints.Add<MarsdemProvider>("/wwtweb/marsdem.aspx", "application/octet-stream");
            endpoints.Add<MarsHiriseProvider>("/wwtweb/MarsHirise.aspx", "image/png");
            endpoints.Add<MarsMocProvider>("/wwtweb/MarsMoc.aspx", "image/png");
            endpoints.Add<MartianTileProvider>("/wwtweb/MartianTile.aspx", "text/plain");
            endpoints.Add<MartianTile2Provider>("/wwtweb/MartianTile2.aspx", "image/png");
            endpoints.Add<MartianTileProvider>("/wwtweb/MartianTileNew.aspx", "text/plain");
            endpoints.Add<MipsgalProvider>("/wwtweb/Mipsgal.aspx", "image/png");
            endpoints.Add<MoondemProvider>("/wwtweb/moondem.aspx", "application/octet-stream");
            endpoints.Add<MoonOctProvider>("/wwtweb/moonOct.aspx", "image/png");
            endpoints.Add<MoontoastProvider>("/wwtweb/moontoast.aspx", "image/png");
            endpoints.Add<MoontoastdemProvider>("/wwtweb/moontoastdem.aspx", "application/octet-stream");
            endpoints.Add<PostMarsProvider>("/wwtweb/postmars.aspx", "text/plain");
            endpoints.Add<PostMarsProvider>("/wwtweb/postmarsdem.aspx", "text/plain");
            endpoints.Add<PostMarsProvider>("/wwtweb/postmarsdem2.aspx", "text/plain");
            endpoints.Add<PostRatingFeedbackProvider>("/wwtweb/PostRatingFeedback.aspx");
            endpoints.Add<RassToastProvider>("/wwtweb/RassToast.aspx", "image/png");
            endpoints.Add<SDSS12ToastProvider>("/wwtweb/SDSS12Toast.aspx", "image/png");
            endpoints.Add<SDSSToastProvider>("/wwtweb/SDSSToast.aspx", "image/png");
            endpoints.Add<SDSSToastOfflineProvider>("/wwtweb/SDSSToast.offline.aspx", "image/png");
            endpoints.Add<SDSSToast2Provider>("/wwtweb/SDSSToast2.aspx", "image/png");
            endpoints.Add<ShowImageProvider>("/wwtweb/ShowImage.aspx", "application/x-wtml");
            endpoints.Add<ShowImage2Provider>("/wwtweb/ShowImage2.aspx", "application/x-wtml");
            endpoints.Add<StarChartProvider>("/wwtweb/StarChart.aspx", "image/png");
            endpoints.Add<TestProvider>("/wwtweb/test.aspx", "text/html");
            endpoints.Add<TestfailoverProvider>("/wwtweb/testfailover.aspx", "text/html");
            endpoints.Add<ThumbnailProvider>("/wwtweb/thumbnail.aspx", "image/jpeg");
            endpoints.Add<TileImageProvider>("/wwtweb/TileImage.aspx", "application/x-wtml");
            endpoints.Add<TilesProvider>("/wwtweb/tiles.aspx", "image/png");
            endpoints.Add<Tiles2Provider>("/wwtweb/tiles2.aspx", "image/png");
            endpoints.Add<TilethumbProvider>("/wwtweb/tilethumb.aspx", "image/jpeg");
            endpoints.Add<TwoMassToastProvider>("/wwtweb/TwoMassToast.aspx", "image/png");
            endpoints.Add<TychoOctProvider>("/wwtweb/TychoOct.aspx", "image/png");
            endpoints.Add<VeblendProvider>("/wwtweb/veblend.aspx", "image/jpeg");
            endpoints.Add<VersionProvider>("/wwtweb/version.aspx", "text/plain");
            endpoints.Add<VersionsProvider>("/wwtweb/versions.aspx", "text/plain");
            endpoints.Add<VlssToastProvider>("/wwtweb/vlssToast.aspx", "image/png");
            endpoints.Add<WebloginProvider>("/wwtweb/weblogin.aspx", "text/plain");
            endpoints.Add<WebServiceProxyProvider>("/wwtweb/WebServiceProxy.aspx", "text/plain");
            endpoints.Add<WmapProvider>("/wwtweb/wmap.aspx", "image/png");
            endpoints.Add<XML2WTTProvider>("/wwtweb/XML2WTT.aspx");

            _endpoints = endpoints;
        }

        private class EndpointManager
        {
            private readonly IServiceProvider _services;
            private readonly Dictionary<string, (Type provider, string contentType)> _map;

            public EndpointManager(IServiceProvider provider)
            {
                _services = provider;
                _map = new Dictionary<string, (Type, string)>(StringComparer.OrdinalIgnoreCase);
            }

            public void Add<T>(string endpoint, string contentType = null)
                where T : RequestProvider
            {
                _map.Add(endpoint, (typeof(T), contentType));
            }

            public ScopedRequest GetRequestScope(string endpoint)
            {
                if (_map.TryGetValue(endpoint, out var result))
                {
                    var scope = _services.CreateScope();
                    var provider = (RequestProvider)scope.ServiceProvider.GetRequiredService(result.provider);

                    return new ScopedRequest(scope, result.contentType, provider);
                }

                return default;
            }
        }

        private class ScopedRequest : IDisposable
        {
            private readonly IServiceScope _scope;

            public ScopedRequest(IServiceScope scope, string contentType, RequestProvider provider)
            {
                _scope = scope;
                ContentType = contentType;
                Provider = provider;
            }

            public string ContentType { get; }

            public RequestProvider Provider { get; }

            public T Resolve<T>() => _scope.ServiceProvider.GetRequiredService<T>();

            public void Dispose()
            {
                _scope.Dispose();
            }
        }
    }
}
