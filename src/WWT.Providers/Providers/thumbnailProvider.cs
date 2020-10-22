using WWTThumbnails;

namespace WWT.Providers
{
    public class ThumbnailProvider : RequestProvider
    {
        private readonly IThumbnailAccessor _thumbnails;

        public ThumbnailProvider(IThumbnailAccessor thumbnails)
        {
            _thumbnails = thumbnails;
        }

        public override void Run(IWwtContext context)
        {
            string name = context.Request.Params["name"];
            string type = context.Request.Params["class"];

            using (var s = _thumbnails.GetThumbnailStream(name, type))
            {
                s.CopyTo(context.Response.OutputStream);
                context.Response.Flush();
                context.Response.End();
            }
        }
    }
}
