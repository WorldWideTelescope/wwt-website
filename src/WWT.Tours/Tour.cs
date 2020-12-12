#nullable disable

using System;

namespace WWT.Tours
{
    public class Tour
    {
        private Guid tourGUID;

        public Tour()
        {

        }

        public double AverageRating { get; set; }

        public Guid TourGuid
        {
            get { return this.tourGUID; }
            set { this.tourGUID = value; }
        }

        public string TourTitle { get; set; }

        public string WorkFlowStatusCode { get; set; }

        public DateTime TourSubmittedDateTime { get; set; }

        public DateTime TourApprovedDateTime { get; set; }

        public DateTime TourRejectedDateTime { get; set; }

        public string TourDescription { get; set; }

        public string TourAttributionAndCredits { get; set; }

        public string AuthorName { get; set; }

        public string AuthorEmailAddress { get; set; }

        public string AuthorURL { get; set; }

        public string AuthorSecondaryEmailAddress { get; set; }

        public string AuthorContactPhoneNumber { get; set; }

        public string AuthorContactText { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationURL { get; set; }

        public string TourKeywordList { get; set; }

        public string TourAstroObjectList { get; set; }

        public string TourITHList { get; set; }

        public string TourExplicitTourLinkList { get; set; }

        public int LengthInSecs { get; set; }

        public string TourXML { get; set; }

        public override string ToString()
        {
            return String.Format("GUID: {0} ; TITLE: {1} ", this.tourGUID.ToString(), this.TourTitle);
        }

    }
}
