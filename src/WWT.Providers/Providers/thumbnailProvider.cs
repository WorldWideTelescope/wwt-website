using WWTThumbnails;

namespace WWT.Providers
{
    public class thumbnailProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            string name = context.Request.Params["name"];
            string type = context.Request.Params["class"];

            using (var s = WWTThumbnail.GetThumbnailStream(name, type))
            {
                s.CopyTo(context.Response.OutputStream);
                context.Response.Flush();
                context.Response.End();
            }
        }
    }
}
