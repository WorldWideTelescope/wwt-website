using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WWT.Providers;

namespace WWT.Web;

public static class WwtEndpointExtensions
{
    public static void MapWwt(this IEndpointRouteBuilder endpoints)
    {
        // Many web infra health checks assume that your server will return
        // a 200 result for the root path, so let's make sure that actually
        // happens.
        var attr = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var version = $"WWT.Web app version {attr?.InformationalVersion ?? "0.0.0-unspecified"}\n";
        endpoints.MapGet("/", () => TypedResults.Ok(version));

        // this URL is requested by the Azure App Service Docker framework
        // to check if the container is running. Azure doesn't care if it
        // 404's, but those 404's do get logged as failures in Application
        // Insights, which we'd like to avoid.
        endpoints.MapGet("/robots933456.txt", () => TypedResults.NoContent());

        endpoints.MapWwtProviders();
        endpoints.MapWwtEndpoints();
        endpoints.MapWwtDeprecatedEndpoints();
    }

    private static IEndpointConventionBuilder MapWwtEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/wwtweb");

        group.MapGet("mandel.aspx", static ([FromQuery] LXY q, IMandelbrot mandelbrot) =>
        {
            return TypedResults.Stream(mandelbrot.CreateMandelbrot(q.Level, q.X, q.Y), "image/jpeg");
        }).WithCacheControl();

        group.MapGet("dss.aspx", static async Task<Results<FileStreamHttpResult, NotFound>> ([FromQuery] LXY q, DSSProvider dss, CancellationToken token) =>
        {
            if (await dss.GetStreamAsync(q.Level, q.X, q.Y, token) is { } stream)
            {
                return TypedResults.Stream(stream, "image/png");
            }

            return TypedResults.NotFound();
        }).WithCacheControl();

        return group;
    }

    private static IEndpointConventionBuilder MapWwtDeprecatedEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var deprecated = endpoints.MapGroup("/wwtweb");

        deprecated.MapGet("/BingDemTile2.aspx", DeprecatedMessaging);
        deprecated.MapGet("/dem.aspx", DeprecatedMessaging);
        deprecated.MapGet("/dembath.aspx", DeprecatedMessaging);
        deprecated.MapGet("/demmars_new.aspx", DeprecatedMessaging);
        deprecated.MapGet("/DSSToast.aspx", DeprecatedMessaging);
        deprecated.MapGet("/DustToast.aspx", DeprecatedMessaging);
        deprecated.MapGet("/EarthBlend.aspx", DeprecatedMessaging);
        deprecated.MapGet("/EarthMerBath.aspx", DeprecatedMessaging);
        deprecated.MapGet("/hAlphaToast.aspx", DeprecatedMessaging);
        deprecated.MapGet("/Hirise.aspx", DeprecatedMessaging);
        deprecated.MapGet("/HiriseDem2.aspx", DeprecatedMessaging);
        deprecated.MapGet("/HiriseDem3.aspx", DeprecatedMessaging);
        deprecated.MapGet("/HiriseDem.aspx", DeprecatedMessaging);
        deprecated.MapGet("/jupiter.aspx", DeprecatedMessaging);
        deprecated.MapGet("/mandel1.aspx", DeprecatedMessaging);
        deprecated.MapGet("/mars.aspx", DeprecatedMessaging);
        deprecated.MapGet("/MartianTile2.aspx", DeprecatedMessaging);
        deprecated.MapGet("/moondem.aspx", DeprecatedMessaging);
        deprecated.MapGet("/moonOct.aspx", DeprecatedMessaging);
        deprecated.MapGet("/RassToast.aspx", DeprecatedMessaging);
        deprecated.MapGet("/SDSSToast2.aspx", DeprecatedMessaging);
        deprecated.MapGet("/TychoOct.aspx", DeprecatedMessaging);
        deprecated.MapGet("/veblend.aspx", DeprecatedMessaging);
        deprecated.MapGet("/vlssToast.aspx", DeprecatedMessaging);
        deprecated.MapGet("/wmap.aspx", DeprecatedMessaging);

        return deprecated;

        static string DeprecatedMessaging(HttpContext context)
        {
            Activity.Current?.SetTag("IsDeprecated", true);

            context.Response.StatusCode = 410;
            return "HTTP/410 Gone\n\nThis endpoint is no longer supported.\nFile an issue at https://github.com/WorldWideTelescope/wwt-website/issues if you still need it.\n";
        }
    }

    private static IEndpointConventionBuilder WithCacheControl(this IEndpointConventionBuilder builder, bool isCacheable = true)
    {
        var value = isCacheable ? new CacheControlHeaderValue { Public = true } : new CacheControlHeaderValue { NoCache = true };

        return builder.AddEndpointFilter((ctx, next) =>
        {
            ctx.HttpContext.Response.GetTypedHeaders().CacheControl = value;
            return next(ctx);
        });
    }

    private static void MapWwtProviders(this IEndpointRouteBuilder endpoints)
    {
        var cache = new ConcurrentDictionaryCache();
        var endpointManager = endpoints.ServiceProvider.GetRequiredService<EndpointManager>();

        var @public = new CacheControlHeaderValue { Public = true };
        var nocache = new CacheControlHeaderValue { NoCache = true };

        foreach (var (endpoint, providerType) in endpointManager)
        {
            var provider = (RequestProvider)ActivatorUtilities.CreateInstance(endpoints.ServiceProvider, providerType);

            endpoints.MapGet(endpoint, (HttpContext ctx, [FromKeyedServices("WWT")] ActivitySource activitySource) =>
            {
                using var activity = activitySource.StartActivity("RequestProvider", ActivityKind.Server);
                activity?.AddBaggage("ProviderName", providerType.FullName);

                ctx.Response.ContentType = provider.ContentType;
                ctx.Response.GetTypedHeaders().CacheControl = provider.IsCacheable ? @public : nocache;

                return provider.RunAsync(new AspNetCoreWwtContext(ctx, cache), ctx.RequestAborted);
            });
        }
    }

    /// <summary>
    /// Provides a strongly typed representation of the LXY query parameter often used as the 'Q' parameter. This will be used
    /// to attempt to parse the parameter, and if it fails, the request will be rejected with an explanation of the failure.
    /// </summary>
    private record struct LXY(int Level, int X, int Y) : IParsable<LXY>
    {
        public static LXY Parse(string s, IFormatProvider provider)
        {
            if (TryParse(s, provider, out var result))
            {
                return result;
            }

            throw new FormatException("Could not parse input for LXY");
        }

        public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out LXY result)
        {
            if (s.Split([','], count: 3, StringSplitOptions.TrimEntries) is [{ } sLevel, { } sX, { } sY])
            {
                if (int.TryParse(sLevel, NumberStyles.Integer, provider, out var level) &&
                    int.TryParse(sX, NumberStyles.Integer, provider, out var x) &&
                    int.TryParse(sY, NumberStyles.Integer, provider, out var y))
                {
                    result = new LXY(level, x, y);
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}
