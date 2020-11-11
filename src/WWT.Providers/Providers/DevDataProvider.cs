using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/data/dev")]
    public class DevDataProvider : RequestProvider
    {
        private const int MaxLevel = 4;

        private readonly IDevDataAccessor _devData;

        public DevDataProvider(IDevDataAccessor devData)
        {
            _devData = devData;
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
                level = MaxLevel;
            }

            using var result = await _devData.GetDevDataAsync(level, token);

            await result.CopyToAsync(context.Response.OutputStream, token);
        }
    }
}
