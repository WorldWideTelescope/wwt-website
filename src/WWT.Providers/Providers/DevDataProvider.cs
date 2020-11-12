using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/v2/data/dev_export")]
    public class DevDataProvider : RequestProvider
    {
        private const int MaxLevel = 4;

        private readonly IDevDataAccessor _devData;
        private readonly ILogger<DevDataProvider> _logger;

        public DevDataProvider(IDevDataAccessor devData, ILogger<DevDataProvider> logger)
        {
            _devData = devData;
            _logger = logger;
        }

        public override string ContentType => ContentTypes.Zip;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            if (!int.TryParse(context.Request.Params["level"], out var level))
            {
                level = MaxLevel;
            }

            if (level > MaxLevel)
            {
                _logger.LogInformation("Level {Level} was requested above max {MaxLevel}", level, MaxLevel);
                level = MaxLevel;
            }

            _logger.LogInformation("Retrieving dev data for datasets up to {Level}", level);

            using var result = await _devData.GetDevDataAsync(level, token);

            await result.CopyToAsync(context.Response.OutputStream, token);
        }
    }
}
