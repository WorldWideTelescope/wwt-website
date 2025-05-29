using Microsoft.IO;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WWT.PlateFiles;

#nullable enable

namespace WWT.Web.Caching;

public readonly partial struct ResponseCacheStreamSerializer(RecyclableMemoryStream stream)
{
    private const int HeaderLength = 8;

    private long ContentOffset
    {
        get
        {
            var before = stream.Position;
            stream.Position = 0;
            Span<byte> header = stackalloc byte[HeaderLength];
            stream.ReadExactly(header);
            stream.Position = before;
            return BinaryPrimitives.ReadInt64LittleEndian(header);
        }
        set
        {
            var before = stream.Position;
            stream.Position = 0;
            var header = stream.GetSpan(HeaderLength);
            BinaryPrimitives.WriteInt64LittleEndian(header, value);
            stream.Advance(HeaderLength);
            stream.Position = before;
        }
    }

    private async ValueTask<ResponseCacheHeaders?> GetResponseDetailsAsync(CancellationToken token)
    {
        if (stream.Length < HeaderLength)
        {
            return null;
        }

        var slice = new StreamSlice(stream, HeaderLength, ContentOffset - HeaderLength);

        return await JsonSerializer.DeserializeAsync(slice, CachedContext.Default.ResponseCacheHeaders, token);
    }

    public async ValueTask<HttpResponseMessage?> DeserializeAsync(CancellationToken token)
    {
        var details = await GetResponseDetailsAsync(token);

        if (details is null)
        {
            return null;
        }

        var message = new HttpResponseMessage
        {
            StatusCode = details.StatusCode,
        };

        var contentOffset = ContentOffset;

        if (contentOffset < stream.Length)
        {
            message.Content = new StreamContent(new StreamSlice(stream, contentOffset));
        }

        foreach (var (name, value) in details.Headers)
        {
            message.Headers.Add(name, value);
        }

        return message;
    }

    public async ValueTask<HttpResponseMessage> SerializeAsync(HttpResponseMessage message, CancellationToken token)
    {
        stream.SetLength(0);

        // Save space for header
        stream.Advance(HeaderLength);

        var details = new ResponseCacheHeaders()
        {
            Headers = message.Headers,
            StatusCode = message.StatusCode,
        };

        await JsonSerializer.SerializeAsync(stream, details, CachedContext.Default.ResponseCacheHeaders, token);

        var contentOffset = ContentOffset = stream.Position;

        if (message.Content is { } content)
        {
            using var contentStream = content.ReadAsStream(token);

            await contentStream.CopyToAsync(stream, token);

            message.Content = new StreamContent(new StreamSlice(stream, contentOffset));
        }

        return message;
    }

    [JsonSerializable(typeof(ResponseCacheHeaders))]
    private sealed partial class CachedContext : JsonSerializerContext
    {
    }

    private sealed class ResponseCacheHeaders
    {
        [JsonPropertyName("c")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonPropertyName("h")]
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; } = [];
    }
}