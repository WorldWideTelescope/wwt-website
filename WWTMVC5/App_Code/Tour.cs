using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace WebServices
{
/// <summary>
/// Summary description for Class1
/// </summary>
/// 
public class Tour
{
    private Guid tourGUID;
    private string workFlowStatusCode;
    private DateTime tourSubmittedDateTime;
    private DateTime tourApprovedDateTime;
    private DateTime tourRejectedDateTime;
    private string tourTitle;
    private string tourDescription;
    private string tourAttributionAndCredits;
    private string authorName;
    private string authorEmailAddress;
    private string authorURL;
    private string authorSecondaryEmailAddress;
    private string authorContactPhoneNumber;
    private string authorContactText;
    private string organizationName;
    private string organizationURL;
    private string tourKeywordList;
    private string tourAstroObjectList;
    private string tourExplicitTourLinkList;
    private int lengthInSecs;
    private string tourXML;
    private double averageRating;
    private int numberOfRatings;
    private int numberOfObjections;

    public Tour()
    {

    }

    public Guid TourGuid
    {
        get { return this.tourGUID; }
        set { this.tourGUID = value; }
    }

    public string TourTitle
    {
        get { return this.tourTitle; }
        set { this.tourTitle = value; }
    }

    public string WorkFlowStatusCode
    {
        get { return this.workFlowStatusCode; }
        set { this.workFlowStatusCode = value; }
    }

    public DateTime TourSubmittedDateTime
    {
        get { return this.tourSubmittedDateTime; }
        set { this.tourSubmittedDateTime = value; }
    }

    public DateTime TourApprovedDateTime
    {
        get { return this.tourApprovedDateTime; }
        set { this.tourApprovedDateTime = value; }
    }

    public DateTime TourRejectedDateTime
    {
        get { return this.tourRejectedDateTime; }
        set { this.tourRejectedDateTime = value; }
    }

    public string TourDescription
    {
        get { return this.tourDescription; }
        set { this.tourDescription = value; }
    }

    public string TourAttributionAndCredits
    {
        get { return this.tourAttributionAndCredits; }
        set { this.tourAttributionAndCredits = value; }
    }

    public string AuthorName
    {
        get { return this.authorName; }
        set { this.authorName = value; }
    }

    public string AuthorEmailAddress
    {
        get { return this.authorEmailAddress; }
        set { this.authorEmailAddress = value; }
    }

    public string AuthorURL
    {
        get { return this.authorURL; }
        set { this.authorURL = value; }
    }

    public string AuthorSecondaryEmailAddress
    {
        get { return this.authorSecondaryEmailAddress; }
        set { this.authorSecondaryEmailAddress = value; }
    }

    public string AuthorContactPhoneNumber
    {
        get { return this.authorContactPhoneNumber; }
        set { this.authorContactPhoneNumber = value; }
    }

    public string AuthorContactText
    {
        get { return this.authorContactText; }
        set { this.authorContactText = value; }
    }

    public string OrganizationName
    {
        get { return this.organizationName; }
        set { this.organizationName = value; }
    }

    public string OrganizationURL
    {
        get { return this.organizationURL; }
        set { this.organizationURL = value; }
    }

    public string TourKeywordList
    {
        get { return this.tourKeywordList; }
        set { this.tourKeywordList = value; }
    }

    public string TourAstroObjectList
    {
        get { return this.tourAstroObjectList; }
        set { this.tourAstroObjectList = value; }
    }

    public string TourExplicitTourLinkList
    {
        get { return this.tourExplicitTourLinkList; }
        set { this.tourExplicitTourLinkList = value; }
    }

    public int LengthInSecs
    {
        get { return this.lengthInSecs; }
        set { this.lengthInSecs = value; }
    }

    public string TourXML
    {
        get { return this.tourXML; }
        set { this.tourXML = value; }
    }

    public double AverageRating
    {
        get { return this.averageRating; }
        set { this.averageRating = value; }
    }

    public int NumberOfRatings
    {
        get { return this.numberOfRatings; }
        set { this.numberOfRatings = value; }
    }

    public int NumberOfObjections
    {
        get { return this.numberOfObjections; }
        set { this.numberOfObjections = value; }
    }

    public override string ToString()
    {
        return String.Format("GUID: {0} ; TITLE: {1} ", this.tourGUID.ToString(), this.tourTitle);
    }

}
}
