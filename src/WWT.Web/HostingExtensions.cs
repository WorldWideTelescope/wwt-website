using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Linq;
using WWT.Providers;
using WWT.Web.Caching;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class HostingExtensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddRequestCaching(options =>
            {
                builder.Configuration.GetSection("Caching").Bind(options);
            });

            // Turn on resilience by default
            http.AddStandardResilienceHandler()
                .SelectPipelineByAuthority();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<IStartupFilter, AddRefererStartupFilter>();
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation())
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddSource(Constants.ActivitySourceName)
                .AddProcessor(new IgnoreFailedBlobProcessor())
                .AddHttpClientInstrumentation());

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        {
            builder.Services.AddOpenTelemetry()
               .UseAzureMonitor();
        }

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }

    /// <summary>
    /// Ensure each request has a tag to track the referer header.
    /// </summary>
    private sealed class AddRefererStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => builder =>
        {
            builder.Use((context, next) =>
            {
                if (Activity.Current is { } activity)
                {
                    activity.AddTag("referer", context.Request.Headers.Referer);
                }

                return next(context);
            });
            next(builder);
        };
    }

    /// <summary>
    /// Don't report missing thumbnails or tours as telemetry errors; that's just how our system works.
    /// </summary>
    private sealed class IgnoreFailedBlobProcessor : BaseProcessor<Activity>
    {
        public override void OnEnd(Activity data)
        {
            if (data.Status == ActivityStatusCode.Error)
            {
                if (data.Status == ActivityStatusCode.Error && data.GetTagItem("error.type") is string error && error == "404" && data.GetTagItem("url.full") is string url)
                {
                    if (url.StartsWith("https://wwtfiles.blob.core.windows.net/thumbnails/", StringComparison.OrdinalIgnoreCase))
                    {
                        data.SetStatus(ActivityStatusCode.Unset);
                        data.Parent?.SetTag("missing_item", "thumbnail");
                    }
                    else if (url.StartsWith("https://wwtfiles.blob.core.windows.net/coretours/", StringComparison.OrdinalIgnoreCase))
                    {
                        data.SetStatus(ActivityStatusCode.Unset);
                        data.Parent?.SetTag("missing_item", "coretours");
                    }
                }
                else if (data.GetTagItem("missing_item") is { })
                {
                    data.SetStatus(ActivityStatusCode.Unset);
                }
            }

            base.OnEnd(data);
        }
    }
}
