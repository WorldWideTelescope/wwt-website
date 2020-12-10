// Cf:
// - https://www.meziantou.net/application-insights-track-http-referer.htm
// - https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-filtering-sampling#add-properties-itelemetryinitializer

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace WWT.Web
{
    public class ExtraTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExtraTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                return;

            var requestTelemetry = telemetry as RequestTelemetry;
            if (requestTelemetry == null)
                return;

            if (context.Request.Headers.TryGetValue("Referer", out var value))
            {
                requestTelemetry.Properties["Referer"] = value.ToString();
            }
        }
    }
}
