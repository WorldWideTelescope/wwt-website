using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    public class GetTourListProvider : GetTourList
    {
        public GetTourListProvider(FilePathOptions options)
            : base(options)
        {
        }

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string etag = context.Request.Headers["If-None-Match"];


            context.Response.ClearHeaders();
            context.Response.Clear();
            context.Response.ContentType = "application/x-wtml";

            string toursXML = null;
            UpdateCacheEx(context.Cache);
            toursXML = (string)context.Cache["WWTXMLTours"];

            if (toursXML != null)
            {
                int version = (int)context.Cache.Get("Version");
                string newEtag = version.ToString();

                if (newEtag != etag)
                {
                    context.Response.AddHeader("etag", newEtag);
                    context.Response.AddHeader("Cache-Control", "no-cache");
                    context.Response.Write(toursXML);
                }
                else
                {
                    context.Response.Status = "304 Not Modified";
                }
            }
            context.Response.End();

            return Task.CompletedTask;
        }

        protected override string SqlCommandString => "Select CategoryId, ParentCatID, Name, CatThumbnailUrl from TourCategories where ParentCatId = ";

        protected override string HierarchySqlCommand => "Select CategoryId, ParentCatID, Name, CatThumbnailUrl from TourCategories where ParentCatId = 0 and CategoryId <> 0";
    }
}
