using Microsoft.Extensions.Logging;
using Open.ChannelExtensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PlateManager
{
    internal class UploadProcessor : ICommand
    {
        private readonly UploadProcessorOptions _options;
        private readonly IEnumerable<IWorkItemGenerator> _generators;
        private readonly ILogger<UploadProcessor> _logger;

        public UploadProcessor(UploadProcessorOptions options, IEnumerable<IWorkItemGenerator> generators, ILogger<UploadProcessor> logger)
        {
            _options = options;
            _generators = generators;
            _logger = logger;
        }

        public Task RunAsync(CancellationToken token)
        {
            int _count = 0;
            int _total = 0;
            
            return Channel.CreateBounded<string>(capacity: 100)
                .Source(_options.Files, token)
                .TransformMany(ProcessFile, capacity: 10000, token: token)
                .ReadAllConcurrentlyAsync(_options.UploaderCount, async action =>
                {
                    try
                    {
                        var count = Interlocked.Increment(ref _count);

                        await action(count, _total, token);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unexpected error running task");
                    }
                });

            IEnumerable<Func<int, int, CancellationToken, Task>> ProcessFile(string file)
            {
                _logger.LogInformation("Adding {File}", file);

                foreach (var generator in _generators)
                {
                    foreach (var task in generator.GenerateWorkItems(file, _options.BaseUrl))
                    {
                        yield return task;

                        Interlocked.Increment(ref _total);
                    }
                }
            }
        }
    }
}
