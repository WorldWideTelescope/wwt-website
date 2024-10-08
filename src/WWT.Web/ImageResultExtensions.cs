using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using System.Threading.Tasks;

namespace WWT.Web;

public static class ImageResultExtensions
{
    public static ImageResult Png(this IResultExtensions _, Image image) => new(image, "image/png", new PngEncoder());

    public static ImageResult Jpeg(this IResultExtensions _, Image image) => new(image, "image/jpeg", new JpegEncoder());
}

public sealed class ImageResult(Image image, string contentType, IImageEncoder encoder) : IResult, IStatusCodeHttpResult, IContentTypeHttpResult
{
    public int? StatusCode => StatusCodes.Status200OK;

    public string ContentType => contentType;

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var manager = httpContext.RequestServices.GetRequiredService<RecyclableMemoryStreamManager>();

        using var stream = manager.GetStream();

        // Copy to local stream so we can get a Content-Length header
        await image.SaveAsync(stream, encoder, httpContext.RequestAborted);

        httpContext.Response.ContentType = contentType;
        httpContext.Response.ContentLength = stream.Length;

        stream.Position = 0;

        await stream.CopyToAsync(httpContext.Response.Body, httpContext.RequestAborted);

        image.Dispose();
    }
}
