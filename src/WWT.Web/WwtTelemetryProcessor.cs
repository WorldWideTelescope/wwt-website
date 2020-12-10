// Cf.: https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-filtering-sampling

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace WWT.Web
{
    public class WwtTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public WwtTelemetryProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        public void Process(ITelemetry item)
        {
            MaybeModify(item);
            this.Next.Process(item);
        }

        private void MaybeModify(ITelemetry item)
        {
            var dep = item as DependencyTelemetry;
            if (dep == null)
                return;

            // Don't report missing thumbnails as App Insights errors. That's just
            // how our system works.

            if (dep.ResultCode == "404" && dep.Data.StartsWith("https://wwtfiles.blob.core.windows.net/thumbnails/"))
                dep.Success = true;
        }
    }
}

