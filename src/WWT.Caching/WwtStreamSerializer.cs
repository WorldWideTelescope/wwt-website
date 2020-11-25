#nullable disable

using Microsoft.Extensions.Logging;
using Swick.Cache;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT
{
    /// <summary>
    /// This implements a serializer and transformer for the caching library to manager Streams. Streams pose two problems while caching:
    /// 
    /// 1. Streams modify state while being read and cannot be reread without resetting the position
    /// 2. Not all streams support the ability to reset position
    /// 
    /// This then hooks into the caching mechanism to transform any stream to a MemoryStream which will then have a byte[] available which is
    /// used during the serialization process. By using a transformer, it can do this copy using async methods and ensure the resulting
    /// stream can be reset.
    /// </summary>
    internal class WwtStreamSerializer : ICacheSerializer, IResultTransformer<Stream>
    {
        private readonly ILogger<WwtStreamSerializer> _logger;

        public WwtStreamSerializer(ILogger<WwtStreamSerializer> logger)
        {
            _logger = logger;
        }

        byte[] ICacheSerializer.GetBytes<T>(T obj)
        {
            if (obj is MemoryStream ms)
            {
                return ms.ToArray();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        T ICacheSerializer.GetValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(Stream))
            {
                return (T)(object)new MemoryStream(data);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        ValueTask<Stream> IResultTransformer<Stream>.ResetAsync(Stream input, CancellationToken token)
        {
            if (input is MemoryStream ms)
            {
                ms.Position = 0;
            }
            else
            {
                throw new NotSupportedException();
            }

            return new ValueTask<Stream>(input);
        }

        async ValueTask<Stream> IResultTransformer<Stream>.TransformAsync(Stream input, CancellationToken token)
        {
            if (input is null)
            {
                _logger.LogInformation("Input to serialize was null.");
                return null;
            }

            if (input is MemoryStream)
            {
                return input;
            }

            _logger.LogInformation("Copying input stream to a buffer to ensure it is done asynchronously");

            using (input)
            {
                var ms = new MemoryStream();
                await input.CopyToAsync(ms, token).ConfigureAwait(false);
                return ms;
            }
        }
    }
}
