#nullable disable

using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/GetTourList.aspx")]
    public class GetTourListProvider : GetTourList
    {
        public GetTourListProvider(WwtOptions options)
            : base(options)
        {
        }

        public override string ContentType => ContentTypes.XWtml;

        public override bool IsCacheable => false;

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string etag = context.Request.Headers["If-None-Match"];
            UpdateCacheEx(context.Cache);
            string toursXML = (string)context.Cache["WWTXMLTours"];

            if (toursXML != null)
            {
                int version = (int)context.Cache.Get("Version");
                string newEtag = version.ToString();

                if (newEtag != etag)
                {
                    context.Response.AddHeader("etag", newEtag);
                    await context.Response.WriteAsync(toursXML, token);
                }
                else
                {
                    await Report304Async(context, token);
                }
            }
            context.Response.End();
        }

        protected override string SqlCommandString => "Select CategoryId, ParentCatID, Name, CatThumbnailUrl from TourCategories where ParentCatId = ";

        protected override string HierarchySqlCommand => "Select CategoryId, ParentCatID, Name, CatThumbnailUrl from TourCategories where ParentCatId = 0 and CategoryId <> 0";
    }
}
