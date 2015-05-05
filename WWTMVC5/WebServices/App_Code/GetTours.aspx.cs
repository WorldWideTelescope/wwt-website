using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using WebServices;

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

    public override string ToString()
    {
        return String.Format("GUID: {0} ; TITLE: {1} ", this.tourGUID.ToString(), this.tourTitle);
    }

}

public partial class GetTours : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public static string GetToursXML()
    {
        List<Tour> SQLTours = new List<Tour>();
        int rc = GetSQLTourArrayList(SQLTours);

        if (SQLTours.Count > 0)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                    xmlWriter.WriteStartElement("Folder");
                    foreach (Tour tr in SQLTours)
                    {
                        xmlWriter.WriteStartElement("Tour");
                        xmlWriter.WriteAttributeString("Title", tr.TourTitle);
                        xmlWriter.WriteAttributeString("ID", tr.TourGuid.ToString());
                        xmlWriter.WriteAttributeString("Description", tr.TourDescription);
                        xmlWriter.WriteAttributeString("Classification", "Other");
                        xmlWriter.WriteAttributeString("AuthorEmail", tr.AuthorEmailAddress);
                        xmlWriter.WriteAttributeString("Author", tr.AuthorName);
                        xmlWriter.WriteAttributeString("AuthorUrl", tr.AuthorURL);
                        xmlWriter.WriteAttributeString("OrganizationUrl", tr.OrganizationURL);
                        xmlWriter.WriteAttributeString("OrganizationName", tr.OrganizationName);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.Close();
                }
                sw.Close();
                return sw.ToString();
            }
        }
        return "";
    }

    public static int GetSQLTourArrayList(List<Tour> sqlTours)
    {
        string strErrorMsg;
        //int version = -1;
        DateTime dtBeginDateTime;
        DateTime dtEndDateTime;
        int OrdCol;
        Guid tourguid;
        string tourtitle;
        string workflowstatuscode;
        DateTime toursubmitteddatetime;
        DateTime tourapproveddatetime;
        DateTime tourrejecteddatetime;
        string tourapprovedrejectedbyname;
        string tourdescription;
        string tourattributionandcredits;
        string authorname;
        string authoremailaddress;
        string authorurl;
        string authorsecondaryemailaddress;
        string authorcontactphonenumber;
        string authorcontacttext;
        string organizationname;
        string organizationurl;
        string tourkeywordlist;
        string tourithlist;
        string tourastroobjectlist;
        string tourexplicittourlinklist;
        int lengthinsecs;
        string tourxml;

        strErrorMsg = "";
        SqlConnection myConnection5 = Database.GetConnectionWWTTours();

        dtBeginDateTime = Convert.ToDateTime("1/1/1900");
        dtEndDateTime = Convert.ToDateTime("1/1/2100");

        try
        {
            myConnection5.Open();

            SqlCommand Cmd = null;
            Cmd = new SqlCommand();
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.CommandTimeout = 20;
            Cmd.Connection = myConnection5;

            Cmd.CommandText = "spGetWWTToursForDateRange";

            SqlParameter CustParm = new SqlParameter("@pBeginDateTime", SqlDbType.DateTime);
            CustParm.Value = dtBeginDateTime;
            Cmd.Parameters.Add(CustParm);

            SqlParameter CustParm2 = new SqlParameter("@pEndDateTime", SqlDbType.DateTime);
            CustParm2.Value = dtEndDateTime;
            Cmd.Parameters.Add(CustParm2);

            System.Data.SqlClient.SqlDataReader MySQLReader;
            MySQLReader = Cmd.ExecuteReader();
            while (MySQLReader.Read())
            {
                OrdCol = MySQLReader.GetOrdinal("TourGUID");
                tourguid = MySQLReader.GetGuid(OrdCol);
                OrdCol = MySQLReader.GetOrdinal("TourTitle");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourtitle = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    tourtitle = null;
                }

                OrdCol = MySQLReader.GetOrdinal("WorkFlowStatusCode");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    workflowstatuscode = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    workflowstatuscode = null;
                }

                OrdCol = MySQLReader.GetOrdinal("TourSubmittedDateTime");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    toursubmitteddatetime = Convert.ToDateTime(MySQLReader.GetSqlDateTime(OrdCol).ToString());
                }
                else
                {
                    toursubmitteddatetime = DateTime.MinValue;
                }

                OrdCol = MySQLReader.GetOrdinal("TourApprovedDateTime");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourapproveddatetime = Convert.ToDateTime(MySQLReader.GetSqlDateTime(OrdCol).ToString());
                }
                else
                {
                    tourapproveddatetime = DateTime.MinValue;
                }

                OrdCol = MySQLReader.GetOrdinal("TourRejectedDateTime");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourrejecteddatetime = Convert.ToDateTime(MySQLReader.GetSqlDateTime(OrdCol).ToString());
                }
                else
                {
                    tourrejecteddatetime = DateTime.MinValue;
                }

                OrdCol = MySQLReader.GetOrdinal("TourApprovedRejectedByName");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourapprovedrejectedbyname = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    tourapprovedrejectedbyname = null;
                }

                OrdCol = MySQLReader.GetOrdinal("TourDescription");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourdescription = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    tourdescription = null;
                }

                OrdCol = MySQLReader.GetOrdinal("TourAttributionAndCredits");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourattributionandcredits = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    tourattributionandcredits = null;
                }

                OrdCol = MySQLReader.GetOrdinal("AuthorName");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    authorname = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    authorname = null;
                }

                OrdCol = MySQLReader.GetOrdinal("AuthorEmailAddress");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    authoremailaddress = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    authoremailaddress = null;
                }

                OrdCol = MySQLReader.GetOrdinal("AuthorURL");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    authorurl = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    authorurl = null;
                }

                OrdCol = MySQLReader.GetOrdinal("AuthorSecondaryEmailAddress");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    authorsecondaryemailaddress = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    authorsecondaryemailaddress = null;
                }

                OrdCol = MySQLReader.GetOrdinal("AuthorContactPhoneNumber");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    authorcontactphonenumber = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    authorcontactphonenumber = null;
                }

                OrdCol = MySQLReader.GetOrdinal("AuthorContactText");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    authorcontacttext = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    authorcontacttext = null;
                }

                OrdCol = MySQLReader.GetOrdinal("OrganizationName");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    organizationname = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    organizationname = null;
                }

                OrdCol = MySQLReader.GetOrdinal("OrganizationURL");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    organizationurl = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    organizationurl = null;
                }

                OrdCol = MySQLReader.GetOrdinal("TourKeywordList");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourkeywordlist = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    tourkeywordlist = null;
                }

                OrdCol = MySQLReader.GetOrdinal("TourITHList");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourithlist = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    tourithlist = null;
                }

                OrdCol = MySQLReader.GetOrdinal("TourAstroObjectList");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourastroobjectlist = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    tourastroobjectlist = null;
                }

                OrdCol = MySQLReader.GetOrdinal("TourExplicitTourLinkList");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    tourexplicittourlinklist = MySQLReader.GetString(OrdCol);
                }
                else
                {
                    tourexplicittourlinklist = null;
                }

                OrdCol = MySQLReader.GetOrdinal("LengthInSecs");
                if (MySQLReader[OrdCol] != DBNull.Value)
                {
                    lengthinsecs = MySQLReader.GetInt32(OrdCol);
                }
                else
                {
                    lengthinsecs = -1;
                }

                //OrdCol = MySQLReader.GetOrdinal("TourXML");
                //if (MySQLReader[OrdCol] != DBNull.Value)
                //{
                //    tourxml = MySQLReader.GetString(OrdCol);
                //}
                //else
                //{
                //    tourxml = null;
                //}



                Tour loadTour = new Tour();

                loadTour.TourGuid = tourguid;
                loadTour.TourTitle = tourtitle;
                if (workflowstatuscode != null)
                { loadTour.WorkFlowStatusCode = workflowstatuscode; }
                if (toursubmitteddatetime != null)
                { loadTour.TourSubmittedDateTime = toursubmitteddatetime; }
                if (tourapproveddatetime != null)
                { loadTour.TourApprovedDateTime = tourapproveddatetime; }
                if (tourrejecteddatetime != null)
                { loadTour.TourRejectedDateTime = tourrejecteddatetime; }

                //loadTour.TourApprovedRejectedByName = tourapprovedrejectedbyname;
                loadTour.TourDescription = tourdescription;
                loadTour.TourAttributionAndCredits = tourattributionandcredits;
                loadTour.AuthorName = authorname;
                loadTour.AuthorEmailAddress = authoremailaddress;
                loadTour.AuthorURL = authorurl;
                loadTour.AuthorSecondaryEmailAddress = authorsecondaryemailaddress;
                loadTour.AuthorContactPhoneNumber = authorcontactphonenumber;
                loadTour.AuthorContactText = authorcontacttext;
                loadTour.OrganizationName = organizationname;
                //loadTour.OrganizationURL = organizationurl;
                loadTour.TourKeywordList = tourkeywordlist;
                //loadTour.TourITHList - tourithlist;
                loadTour.TourAstroObjectList = tourastroobjectlist;
                loadTour.TourExplicitTourLinkList = tourexplicittourlinklist;
                loadTour.LengthInSecs = lengthinsecs;
                //loadTour.TourXML = tourxml;

                sqlTours.Add(loadTour);
            }

        }
        catch (InvalidCastException)
        { }

        catch (Exception ex)
        {
            //throw ex.GetBaseException();
            strErrorMsg = ex.Message;
            return -1;
        }
        finally
        {
            if (myConnection5.State == ConnectionState.Open)
                myConnection5.Close();
        }

        return 0;
    }

    public static int GetSQLToursVersion()
    {
        string strErrorMsg;
        int version = -1;

        strErrorMsg = "";
        SqlConnection myConnection5 = Database.GetConnectionWWTTours();

        try
        {
            myConnection5.Open();

            SqlCommand Cmd = null;
            Cmd = new SqlCommand();
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.CommandTimeout = 20;
            Cmd.Connection = myConnection5;

            Cmd.CommandText = "spGetTourVersion";

            System.Data.SqlClient.SqlDataReader MySQLReader;
            MySQLReader = Cmd.ExecuteReader();
            MySQLReader.Read();
            int OrdVersionNumber = MySQLReader.GetOrdinal("VersionNumber");
            version = MySQLReader.GetInt32(OrdVersionNumber);

        }
        catch (Exception ex)
        {
            //throw ex.GetBaseException();
            strErrorMsg = ex.Message;
        }
        finally
        {
            if (myConnection5.State == ConnectionState.Open)
                myConnection5.Close();
        }

        return version;
    }

    public static string GetToursXML(List<Tour> SQLTours)
    {
        if (SQLTours.Count > 0)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                    xmlWriter.WriteStartElement("Folder");
                    foreach (Tour tr in SQLTours)
                    {
                        xmlWriter.WriteStartElement("Tour");
                        xmlWriter.WriteAttributeString("Title", tr.TourTitle);
                        xmlWriter.WriteAttributeString("ID", tr.TourGuid.ToString());
                        xmlWriter.WriteAttributeString("Description", tr.TourDescription);
                        xmlWriter.WriteAttributeString("Classification", "Other");
                        xmlWriter.WriteAttributeString("AuthorEmail", tr.AuthorEmailAddress);
                        xmlWriter.WriteAttributeString("Author", tr.AuthorName);
                        xmlWriter.WriteAttributeString("AuthorUrl", tr.AuthorURL);
                        xmlWriter.WriteAttributeString("OrganizationUrl", tr.OrganizationURL);
                        xmlWriter.WriteAttributeString("OrganizationName", tr.OrganizationName);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.Close();
                }
                sw.Close();
                return sw.ToString();
            }
        }
        return "";
    }

    public static int UpdateCache()
    {
        bool needToBuild;
        bool cacheIsEmpty;
        int fromCacheVersion;
        int fromSQLVersion;
        int MinutesToAdd;

        List<Tour> SQLTours = new List<Tour>();

        cacheIsEmpty = false;
        needToBuild = false;

        DateTime fromCacheDateTime;
        
        if (HttpContext.Current.Cache.Get("WWTXMLTours") == null)
        {
            cacheIsEmpty = true;
            needToBuild = true;
        }
        // see if you need to build the cache.... 

        // if it has been more than n minutes since you last checked the version, then 
        //  get the version number from sql.   if different, then needtoupdate.  
        try
        {
            fromCacheDateTime = (DateTime)HttpContext.Current.Cache.Get("LastCacheUpdateDateTime");
        }
        catch
        {
            fromCacheDateTime = System.DateTime.Now.AddDays(-1);
        }



        try
        {
            MinutesToAdd = Int32.Parse(ConfigurationManager.AppSettings["TourVersionCheckIntervalMinutes"]);
        }
        catch
        {
            MinutesToAdd = 5;  // if missing config, set to 5 minutes
        }

        if (System.DateTime.Now > fromCacheDateTime.AddMinutes(MinutesToAdd))
        {
            try
            {
                fromCacheVersion = (int)HttpContext.Current.Cache.Get("Version");
            }
            catch
            {
                fromCacheVersion = 0;
            }

            if (fromCacheVersion == 0)
            {
                needToBuild = true;
            }
            else
            {
                fromSQLVersion = GetSQLToursVersion();

                if (fromSQLVersion != fromCacheVersion)
                {
                    needToBuild = true;
                }
                // at this point, you have checked the db to see if the version has changed, you don't need to do this again for the next n minutes
                HttpContext.Current.Cache.Remove("LastCacheUpdateDateTime");
                HttpContext.Current.Cache.Add("LastCacheUpdateDateTime", System.DateTime.Now, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            }

        }
        if (needToBuild)
        {

            // if needToBuild, get the tours from SQL, replace the cache
            // step thru sql result set, create array of tours
            // clear cache
            // add array to cache

            int rc = GetSQLTourArrayList(SQLTours);

            //update the version number in the cache (datetime is already updated)
            fromSQLVersion = GetSQLToursVersion();
            HttpContext.Current.Cache.Remove("Version");
            HttpContext.Current.Cache.Add("Version", fromSQLVersion, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

            //update the WWTTours cache with the SQLTours ArrayList
            HttpContext.Current.Cache.Remove("WWTXMLTours");
            HttpContext.Current.Cache.Add("WWTXMLTours", GetToursXML(SQLTours), null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
        }
        return 0;
    }
}
