using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WWT
{
    /// <summary>
    /// A decorated <see cref="IDistributedCache"/> that enables AppInsights tracking of values being set/retrieved
    /// </summary>
    /// <remarks>
    /// Inspired by discussion here: <see cref="https://stackoverflow.com/questions/49925531/tracking-azure-redis-via-end-to-end-transaction-in-application-insights"/>.
    /// </remarks>
    public class AppInsightsDistributedCache : IDistributedCache
    {
        private readonly object _sentinal = new object();
        private readonly TelemetryClient _client;
        private readonly IDistributedCache _other;

        public AppInsightsDistributedCache(IOptions<TelemetryConfiguration> configuration, IDistributedCache other)
        {
            _client = new TelemetryClient(configuration.Value);
            _other = other;
        }

        public byte[] Get(string key)
          => CreateOperationInternal(key, () => new ValueTask<byte[]>(_other.Get(key))).GetAwaiter().GetResult();

        public Task<byte[]> GetAsync(string key, CancellationToken token = default)
          => CreateOperationInternal(key, async () => await _other.GetAsync(key, default)).AsTask();

        public void Refresh(string key) => throw new System.NotImplementedException();

        public Task RefreshAsync(string key, CancellationToken token = default) => throw new System.NotImplementedException();

        public void Remove(string key) => throw new System.NotImplementedException();

        public Task RemoveAsync(string key, CancellationToken token = default) => throw new System.NotImplementedException();

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
            => CreateOperationInternal(key, () =>
            {
                _other.Set(key, value, options);
                return new ValueTask<object>(_sentinal);
            }).GetAwaiter().GetResult();

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
            => CreateOperationInternal(key, async () =>
            {
                await _other.SetAsync(key, value, options, token).ConfigureAwait(false);
                return _sentinal;
            }).AsTask();

        private async ValueTask<T> CreateOperationInternal<T>(string key, Func<ValueTask<T>> action, [CallerMemberName] string name = null)
        {
            var dependency = new DependencyTelemetry()
            {
                Type = "Redis",
                Name = name,
            };

            dependency.Properties["key"] = key;

            using (_client.StartOperation(dependency))
            {
                try
                {
                    var result = await action().ConfigureAwait(false);
                    dependency.Success = true;
                    return result;
                }
                catch (Exception)
                {
                    dependency.Success = false;
                    throw;
                }
            }
        }
    }
}
