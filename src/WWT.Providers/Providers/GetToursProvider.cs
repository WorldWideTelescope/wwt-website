using System;
using System.Data;
using System.Xml;
using WWTWebservices;

namespace WWT.Providers
{
    public class GetToursProvider : GetTourList
    {
        public GetToursProvider(FilePathOptions options)
            : base(options)
        {
        }

        public override void Run(IWwtContext context)
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
        }

        protected override void LoadTourFromRow(DataRow dr, Tour tr)
        {
            tr.TourITHList = Convert.ToString(dr["TourITHList"]);
            tr.TourAstroObjectList = Convert.ToString(dr["TourAstroObjectList"]);
            tr.TourKeywordList = Convert.ToString(dr["TourKeywordList"]);
            tr.TourExplicitTourLinkList = Convert.ToString(dr["TourExplicitTourLinkList"]);
        }

        protected override void WriteTour(XmlWriter xmlWriter, Tour tr)
        {
            xmlWriter.WriteAttributeString("ITHList", tr.TourITHList);
            xmlWriter.WriteAttributeString("AstroObjectsList", tr.TourAstroObjectList);
            xmlWriter.WriteAttributeString("Keywords", tr.TourKeywordList);
            xmlWriter.WriteAttributeString("RelatedTours", tr.TourExplicitTourLinkList);
        }

        protected override string SqlCommandString => "exec spGetSubCatDetailsFromParCatId ";

        protected override string HierarchySqlCommand => "exec spGetSubCatDetailsForRootCat";
    }
}
