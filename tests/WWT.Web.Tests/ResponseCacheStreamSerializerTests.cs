using Microsoft.IO;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WWT.Web.Caching.Tests;

public class ResponseCacheStreamSerializerTests
{
    [Fact]
    public async Task EmptyStream()
    {
        // Arrange
        var manager = new RecyclableMemoryStreamManager();
        using var stream = manager.GetStream();
        var serializer = new ResponseCacheStreamSerializer(stream);

        // Act
        using var result = await serializer.DeserializeAsync(default);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task StatusCodeTest()
    {
        // Arrange
        var manager = new RecyclableMemoryStreamManager();
        using var stream = manager.GetStream();
        var serializer = new ResponseCacheStreamSerializer(stream);
        using var expected = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        using var result = await serializer.SerializeAsync(expected, default);
        using var deserialized = await serializer.DeserializeAsync(default);

        // Assert
        Assert.Same(expected, result);
        Assert.Equal(expected.StatusCode, deserialized.StatusCode);
    }

    [Fact]
    public async Task HeaderTest()
    {
        // Arrange
        const string HeaderName = "Header";
        const string HeaderValue = "value";

        var manager = new RecyclableMemoryStreamManager();
        using var stream = manager.GetStream();
        var serializer = new ResponseCacheStreamSerializer(stream);
        using var expected = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Headers =
            {
                { HeaderName, HeaderValue }
            }
        };

        // Act
        using var result = await serializer.SerializeAsync(expected, default);
        using var deserialized = await serializer.DeserializeAsync(default);

        // Assert
        Assert.Same(expected, result);
        Assert.Collection(
            deserialized.Headers,
            e =>
            {
                Assert.Equal(HeaderName, e.Key);
                Assert.Equal([HeaderValue], e.Value);
            });
    }

    [Fact]
    public async Task EmptyContent()
    {
        // Arrange
        var manager = new RecyclableMemoryStreamManager();
        using var stream = manager.GetStream();
        var serializer = new ResponseCacheStreamSerializer(stream);
        using var expected = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        using var result = await serializer.SerializeAsync(expected, default);
        using var deserialized = await serializer.DeserializeAsync(default);

        // Assert
        Assert.Same(expected, result);
        Assert.Empty(GetBytes(deserialized.Content));
    }

    [Fact]
    public async Task Content()
    {
        // Arrange
        var content = new byte[] { 1, 2, 3, 4 };
        var manager = new RecyclableMemoryStreamManager();
        using var stream = manager.GetStream();
        var serializer = new ResponseCacheStreamSerializer(stream);
        using var expected = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(new MemoryStream(content))
        };

        // Act
        using var result = await serializer.SerializeAsync(expected, default);
        using var deserialized = await serializer.DeserializeAsync(default);

        // Assert
        Assert.Same(expected, result);
        Assert.NotNull(deserialized.Content);
        Assert.Equal(content, GetBytes(deserialized.Content));
    }

    private byte[] GetBytes(HttpContent content)
    {
        var ms = new MemoryStream();
        using var stream = content.ReadAsStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}