using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace WWT.Web;

public static class UserAgentExtensions
{
    public static void UseUserAgentFiltering(this IApplicationBuilder app)
    {
        app.Use((ctx, next) =>
        {
            if (ctx.GetEndpoint()?.Metadata.GetMetadata<DenyWget>() is { } metadata)
            {
                foreach (var agent in ctx.Request.Headers.UserAgent)
                {
                    if (agent.Contains("wget", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Response.StatusCode = 400;
                        return ctx.Response.WriteAsync("Wget is not allowed to bulk download images. Please contact https://github.com/WorldWideTelescope/wwt-website for more information.");
                    }
                }
            }

            return next(ctx);
        });
    }

    public static T DisallowWget<T>(this T endpoint) where T : IEndpointConventionBuilder => endpoint.WithMetadata(new DenyWget());

    private sealed class DenyWget();
}