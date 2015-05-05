using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


/// <summary>
/// Summary description for WWTWebService
/// </summary>
[WebService(Namespace = "http://research.microsoft.com/WWT/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class WWTWebService : System.Web.Services.WebService {

    public WWTWebService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 

    }

    public class Tour
    {
        private Guid tourGUID;
        private string  workFlowStatusCode;
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
            get { return this.authorURL;}
            set { this.authorURL = value; }}

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


    public static string GetTourGUIDFromXML(string TourXML)
    {
        string TourGUIDString = "failname"; 

        XmlDocument doc = new XmlDocument();

        try
        {
            doc.LoadXml(TourXML);
        }
        catch (Exception ex)
        {
            return TourGUIDString;
        }

        XmlNamespaceManager NamespaceManager = new XmlNamespaceManager(doc.NameTable);

        XmlNode xmlNode = doc.SelectSingleNode("/ImportTourRequest/Tour/TourGUID", NamespaceManager);

        TourGUIDString = xmlNode.InnerText;


        return TourGUIDString;
    }

    public static string GetToursUNC()
    {
        string UNCString = null;
        UNCString = ConfigurationManager.AppSettings["WWTToursTourFileUNC"];
        return UNCString;
    }

    public static SqlConnection GetConnectionAstroObjects()
    {
        string connStr = null;
        connStr = ConfigurationManager.AppSettings["AstroObjectsDBConnectionString"];
        SqlConnection myConnection = null;
        myConnection = new SqlConnection(connStr);
        return myConnection;
    }

    public static SqlConnection GetConnectionWWTTours()
    {
        string connStr = null;
        connStr = ConfigurationManager.AppSettings["WWTToursDBConnectionString"];
        SqlConnection myConnection = null;
        myConnection = new SqlConnection(connStr);
        return myConnection;
    }

    public static int UpdateCache()
    {
        bool needToBuild;
        bool cacheIsEmpty;
        int fromCacheVersion;
        int fromSQLVersion;
        int MinutesToAdd;

        ArrayList SQLTours = new ArrayList();


        cacheIsEmpty = false;
        needToBuild = false;

        DateTime fromCacheDateTime;

        if (HttpContext.Current.Cache.Get("WWTTours") == null)
        {
            cacheIsEmpty = true;
            needToBuild = true;
        }
        // see if you need to build the cache.... 

        // if it has been more than 5 minutes since you last checked the version, then 
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
                HttpContext.Current.Cache.Add("LastCacheUpdateDateTime",System.DateTime.Now, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null); 
            }

        }
        if (needToBuild)
        {

        // if needToBuild, get the tours from SQL, replace the cache


        // step thru sql result set, create array of tours

        // clear cache

        // add array to cache

        //    Guid testguid = new Guid("4B0392B9-050E-414B-A22F-9CD9BFCCB579");

        //    Tour x = new Tour();
        //    x.TourGuid = testguid;
        //    x.TourTitle = "howdy";

            int rc = GetSQLTourArrayList(SQLTours);

            //update the version number in the cache (datetime is already updated)
            fromSQLVersion = GetSQLToursVersion();
            HttpContext.Current.Cache.Remove("Version");
            HttpContext.Current.Cache.Add("Version", fromSQLVersion, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

            //update the WWTTours cache with the SQLTours ArrayList
            HttpContext.Current.Cache.Remove("WWTTours");
            HttpContext.Current.Cache.Add("WWTTours", SQLTours, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null); 

        }
        return 0; 
    }

    public static ArrayList GetCache()
    {
        ArrayList myTours = new ArrayList();
        
        //myTours.Add(new Tour()); 

        //Tour  fromCache  = new Tour();
        myTours = (ArrayList)HttpContext.Current.Cache.Get("WWTTours");
        return myTours;
    }

    public static int GetSQLTourArrayList(ArrayList sqlTours)
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
        SqlConnection myConnection5 = GetConnectionWWTTours();

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

                    OrdCol = MySQLReader.GetOrdinal("TourXML");
                    if (MySQLReader[OrdCol] != DBNull.Value)
                    {
                        tourxml = MySQLReader.GetString(OrdCol);
                    }
                    else
                    {
                        tourxml = null;
                    }



                Tour loadTour = new Tour();

                loadTour.TourGuid = tourguid;
                loadTour.TourTitle = tourtitle;
                if (workflowstatuscode != null)
                    { loadTour.WorkFlowStatusCode = workflowstatuscode; }
                if (toursubmitteddatetime != null)
                    { loadTour.TourSubmittedDateTime = toursubmitteddatetime; }
                if (tourapproveddatetime != null)
                    {loadTour.TourApprovedDateTime = tourapproveddatetime; }
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
                loadTour.TourXML = tourxml;

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
            SqlConnection myConnection5 = GetConnectionWWTTours();

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


    [WebMethod (CacheDuration = 120)]
    public AstroObjectsDataset GetAstroObjectsByRaDec(float Ra, float Dec, float PlusMinusArcSecs)
    {

        AstroObjectDataByRaDec x2 = new AstroObjectDataByRaDec(Ra, Dec, PlusMinusArcSecs);

        AstroObjectsDataset dsAstroObjectData = x2.dsAstroObjectData;

        return dsAstroObjectData;  //"pj return the ds : " + AstroObjectName;
    }
    class AstroObjectDataByRaDec
    {
        private AstroObjectsDataset ds;
        private string strErrorMsg;

        public AstroObjectDataByRaDec(float Ra, float Dec, float PlusMinusArcSecs)
        {
            strErrorMsg = "";
            SqlConnection myConnection2 = GetConnectionAstroObjects();

            try
            {

                myConnection2.Open();
                SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetAstroObjects", myConnection2);

                Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter CustParm = new SqlParameter("@pRa", SqlDbType.Float);
                CustParm.Value = Ra;
                Cmd2.SelectCommand.Parameters.Add(CustParm);

                SqlParameter CustParm2 = new SqlParameter("@pDec", SqlDbType.Float);
                CustParm2.Value = Dec;
                Cmd2.SelectCommand.Parameters.Add(CustParm2);

                SqlParameter CustParm3 = new SqlParameter("@pPlusMinusArcSecs", SqlDbType.Float);
                CustParm3.Value = PlusMinusArcSecs;
                Cmd2.SelectCommand.Parameters.Add(CustParm3);

                ds = new AstroObjectsDataset();

                Cmd2.Fill(ds,ds.Tables[0].TableName);
            }
            catch (Exception ex)
            {
                //throw ex.GetBaseException();
                strErrorMsg = ex.Message;
            }
            finally
            {
                if (myConnection2.State == ConnectionState.Open)
                    myConnection2.Close();
            }


            }

  

        public AstroObjectsDataset dsAstroObjectData
        { get { return ds; } }
        public string ErrorMsg
        { get { return strErrorMsg; } }
    }




    [WebMethod(CacheDuration = 120)]
    public AstroObjectsDataset GetAstroObjectsByName(string AstroObjectName)
    {
        AstroObjectData x1 = new AstroObjectData(AstroObjectName);

        AstroObjectsDataset dsAstroObjectData = x1.dsAstroObjectData;

        return dsAstroObjectData;  //"pj return the ds : " + AstroObjectName;
    }

    class AstroObjectData
    {
        private AstroObjectsDataset ds;
        private string strErrorMsg;

        public AstroObjectData(string AstroObjectName)
        {
            strErrorMsg = "";
            SqlConnection myConnection2 = GetConnectionAstroObjects();

            try
            {

                myConnection2.Open();
                SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetAstroObjects", myConnection2);

                Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter CustParm = new SqlParameter("@pAstroObjectName", SqlDbType.NVarChar);
                CustParm.Value = AstroObjectName;
                Cmd2.SelectCommand.Parameters.Add(CustParm);

                ds = new AstroObjectsDataset();

                Cmd2.Fill(ds,ds.Tables[0].TableName);
            }
            catch (Exception ex)
            {
                //throw ex.GetBaseException();
                strErrorMsg = ex.Message;
            }
            finally
            {
                if (myConnection2.State == ConnectionState.Open)
                    myConnection2.Close();
            }


            }

  

        public AstroObjectsDataset dsAstroObjectData
        { get { return ds; } }
        public string ErrorMsg
        { get { return strErrorMsg; } }
    }


    [WebMethod(CacheDuration = 120)]
    public AstroObjectsDataset GetAstroObjectsInCatalog(string CatalogName)
    {

        AstroObjectDataInCatalog x3 = new AstroObjectDataInCatalog(CatalogName);

        AstroObjectsDataset dsAstroObjectData = x3.dsAstroObjectData;

        return dsAstroObjectData;  //"pj return the ds : " + AstroObjectName;
    }
    class AstroObjectDataInCatalog
    {
        private AstroObjectsDataset ds;
        private string strErrorMsg;

        public AstroObjectDataInCatalog(string CatalogName)
        {
            strErrorMsg = "";
            SqlConnection myConnection4 = GetConnectionAstroObjects();

            try
            {

                myConnection4.Open();
                SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetCatalog", myConnection4);

                Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter CustParm = new SqlParameter("@pCatalogName", SqlDbType.VarChar);
                CustParm.Value = CatalogName;
                Cmd2.SelectCommand.Parameters.Add(CustParm);

                ds = new AstroObjectsDataset();

                Cmd2.Fill(ds, ds.Tables[0].TableName);
            }
            catch (Exception ex)
            {
                //throw ex.GetBaseException();
                strErrorMsg = ex.Message;
            }
            finally
            {
                if (myConnection4.State == ConnectionState.Open)
                    myConnection4.Close();
            }


        }



        public AstroObjectsDataset dsAstroObjectData
        { get { return ds; } }
        public string ErrorMsg
        { get { return strErrorMsg; } }
    }

    [WebMethod]
    public WWTToursDataset ImportTour(string TourXML, byte[] TourBlob, byte[] TourThumbnail, byte[] AuthorThumbnail)
    {

        WWTTour x4 = new WWTTour(TourXML, TourBlob, TourThumbnail, AuthorThumbnail);

        WWTToursDataset dsWWTTourData = x4.dsWWTTourData;

        return dsWWTTourData;  //"pj return the ds : " + AstroObjectName;
    }
    class WWTTour
    {
        private WWTToursDataset ds;
        private string strErrorMsg;

        public WWTTour(string TourXML, byte[] TourBlob, byte[] TourThumbnail, byte[] AuthorThumbnail)
        {
            strErrorMsg = "";
            SqlConnection myConnection5 = GetConnectionWWTTours();

            //byte[] testerByte = new byte[] { 66, 67, 68 };

            try
            {
                // Tour Thumbnail

                string strTourUNC = GetToursUNC();
                string TourGUIDString = GetTourGUIDFromXML(TourXML);
                string NewFileName = "fail";

                if (TourThumbnail.Length > 0)
                {
                    NewFileName = strTourUNC + "\\" + TourGUIDString + "_TourThumb.bin";
                    Stream t = new FileStream(NewFileName, FileMode.Create);
                    BinaryWriter b = new BinaryWriter(t);
                    b.Write(TourThumbnail);
                    t.Close();
                }

                // Tour Blob
                if (TourBlob.Length > 0)
                {
                    NewFileName = strTourUNC + "\\" + TourGUIDString + ".bin";
                    Stream t2 = new FileStream(NewFileName, FileMode.Create);
                    BinaryWriter b2 = new BinaryWriter(t2);
                    b2.Write(TourBlob);
                    t2.Close();
                }


                // Author Thumbnail

                if (AuthorThumbnail.Length > 0)
                {
                    NewFileName = strTourUNC + "\\" + TourGUIDString + "_AuthorThumb.bin";
                    Stream t3 = new FileStream(NewFileName, FileMode.Create);
                    BinaryWriter b3 = new BinaryWriter(t3);
                    b3.Write(AuthorThumbnail);
                    t3.Close();
                }

            }
            catch (Exception ex)
            {
                return; 
            }


            try
            {

                myConnection5.Open();
                SqlDataAdapter Cmd2 = new SqlDataAdapter("spImportTour", myConnection5);

                Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter CustParm = new SqlParameter("@pInputXMLStream", SqlDbType.NVarChar);
                CustParm.Value = TourXML;
                Cmd2.SelectCommand.Parameters.Add(CustParm);

                ds = new WWTToursDataset();

                Cmd2.Fill(ds, ds.Tables[0].TableName);
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


        }



        public WWTToursDataset dsWWTTourData
        { get { return ds; } }
        public string ErrorMsg
        { get { return strErrorMsg; } }
    }


    [WebMethod(CacheDuration = 120)]
    public WWTToursDataset GetToursForGUID(string TourGUID)
    {

        WWTTourForGUID x4 = new WWTTourForGUID(TourGUID);

        WWTToursDataset dsWWTTourData = x4.dsWWTTourData;

        return dsWWTTourData;  //"pj return the ds : " + AstroObjectName;
    }
    class WWTTourForGUID
    {
        private WWTToursDataset ds;
        private string strErrorMsg;

        public WWTTourForGUID(string TourGUID)
        {
            strErrorMsg = "";
            SqlConnection myConnection5 = GetConnectionWWTTours();
            
            try
            {
                int x = UpdateCache();

                ArrayList whatiscache = GetCache();


                myConnection5.Open();
                SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetWWTToursForGUID", myConnection5);

                Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter CustParm = new SqlParameter("@pWWTTourGUID", SqlDbType.VarChar);
                CustParm.Value = TourGUID;
                Cmd2.SelectCommand.Parameters.Add(CustParm);

                ds = new WWTToursDataset();

                Cmd2.Fill(ds, ds.Tables[0].TableName);
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


        }



        public WWTToursDataset dsWWTTourData
        { get { return ds; } }
        public string ErrorMsg
        { get { return strErrorMsg; } }
    }

    [WebMethod]
    public WWTToursDataset UpdateTourWorkFlowStatus(string TourGUID, char WorkFlowStatusCode, string ApprovedRejectedByName)
    {

        WWTTourUpdtStatus x4 = new WWTTourUpdtStatus(TourGUID, WorkFlowStatusCode, ApprovedRejectedByName);

        WWTToursDataset dsWWTTourData = x4.dsWWTTourData;

        return dsWWTTourData;  //"pj return the ds : " + AstroObjectName;
    }
    class WWTTourUpdtStatus
    {
        private WWTToursDataset ds;
        private string strErrorMsg;

        public WWTTourUpdtStatus(string TourGUID, char WorkFlowStatusCode, string ApprovedRejectedByName)
        {
            strErrorMsg = "";
            SqlConnection myConnection5 = GetConnectionWWTTours();

            try
            {

                myConnection5.Open();
                SqlDataAdapter Cmd2 = new SqlDataAdapter("spUpdateTourWorkFlowStatus", myConnection5);

                Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter CustParm = new SqlParameter("@pWWTTourGUID", SqlDbType.VarChar);
                CustParm.Value = TourGUID;
                Cmd2.SelectCommand.Parameters.Add(CustParm);

                SqlParameter CustParm2 = new SqlParameter("@pWorkFlowStatusCode", SqlDbType.Char);
                CustParm2.Value = WorkFlowStatusCode;
                Cmd2.SelectCommand.Parameters.Add(CustParm2);

                SqlParameter CustParm3 = new SqlParameter("@pTourApprovedRejectedByName", SqlDbType.NVarChar);
                CustParm3.Value = ApprovedRejectedByName;
                Cmd2.SelectCommand.Parameters.Add(CustParm3);

                ds = new WWTToursDataset();

                Cmd2.Fill(ds, ds.Tables[0].TableName);
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


        }



        public WWTToursDataset dsWWTTourData
        { get { return ds; } }
        public string ErrorMsg
        { get { return strErrorMsg; } }
    }





    [WebMethod(CacheDuration = 120)]
    public WWTToursDataset GetToursForAuthor(string AuthorName)
    {

        WWTTourForAuthor x4 = new WWTTourForAuthor(AuthorName);

        WWTToursDataset dsWWTTourData = x4.dsWWTTourData;

        return dsWWTTourData;  //"pj return the ds : " + AstroObjectName;
    }
    class WWTTourForAuthor
    {
        private WWTToursDataset ds;
        private string strErrorMsg;

        public WWTTourForAuthor(string AuthorName)
        {
            strErrorMsg = "";
            SqlConnection myConnection5 = GetConnectionWWTTours();

            try
            {
                int x = UpdateCache();

                ArrayList whatiscache = GetCache();

                myConnection5.Open();
                SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetWWTToursForAuthor", myConnection5);

                Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter CustParm = new SqlParameter("@pAuthorName", SqlDbType.NVarChar);
                CustParm.Value = AuthorName;
                Cmd2.SelectCommand.Parameters.Add(CustParm);

                ds = new WWTToursDataset();

                Cmd2.Fill(ds, ds.Tables[0].TableName);
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


        }



        public WWTToursDataset dsWWTTourData
        { get { return ds; } }
        public string ErrorMsg
        { get { return strErrorMsg; } }
    }




    [WebMethod(CacheDuration = 120)]
    public WWTToursDataset GetToursForKeyword(string Keyword)
    {

        WWTTourForKeyword x4 = new WWTTourForKeyword(Keyword);

        WWTToursDataset dsWWTTourData = x4.dsWWTTourData;

        return dsWWTTourData;  //"pj return the ds : " + AstroObjectName;
    }
    class WWTTourForKeyword
    {
        private WWTToursDataset ds;
        private string strErrorMsg;

        public WWTTourForKeyword(string Keyword)
        {

            int x = UpdateCache();

            ArrayList whatiscache = GetCache();

            strErrorMsg = "";
            SqlConnection myConnection5 = GetConnectionWWTTours();

            try
            {

                myConnection5.Open();
                SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetWWTToursForKeyword", myConnection5);

                Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter CustParm = new SqlParameter("@pKeyword", SqlDbType.NVarChar);
                CustParm.Value = Keyword;
                Cmd2.SelectCommand.Parameters.Add(CustParm);

                ds = new WWTToursDataset();

                Cmd2.Fill(ds, ds.Tables[0].TableName);
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


        }



        public WWTToursDataset dsWWTTourData
        { get { return ds; } }
        public string ErrorMsg
        { get { return strErrorMsg; } }
    }


    [WebMethod(CacheDuration = 120)]
    public WWTToursDataset GetToursForDateRange(string BeginDateTime, string EndDateTime)
    {

        WWTTourForDateRange x4 = new WWTTourForDateRange(BeginDateTime, EndDateTime);

        WWTToursDataset dsWWTTourData = x4.dsWWTTourData;

        return dsWWTTourData;  //"pj return the ds : " + AstroObjectName;
    }
    class WWTTourForDateRange
    {
        private WWTToursDataset ds;
        private string strErrorMsg;

        public WWTTourForDateRange(string BeginDateTime, string EndDateTime)
        {
            strErrorMsg = "";
            DateTime dtBeginDateTime;
            DateTime dtEndDateTime;

            SqlConnection myConnection5 = GetConnectionWWTTours();
            try
            {
                    dtBeginDateTime = Convert.ToDateTime(BeginDateTime);
            }
            catch
            {
                dtBeginDateTime = Convert.ToDateTime("1/1/1900");
            }

            try
            {
                dtEndDateTime = Convert.ToDateTime(EndDateTime);
            }
            catch
            {
                dtEndDateTime = Convert.ToDateTime("1/1/2100");
            }


            try
            {
                int x = UpdateCache();

                ArrayList whatiscache = GetCache();

                myConnection5.Open();
                SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetWWTToursForDateRange", myConnection5);

                Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter CustParm = new SqlParameter("@pBeginDateTime", SqlDbType.DateTime);
                CustParm.Value = dtBeginDateTime;
                Cmd2.SelectCommand.Parameters.Add(CustParm);

                SqlParameter CustParm2 = new SqlParameter("@pEndDateTime", SqlDbType.DateTime);
                CustParm2.Value = dtEndDateTime;
                Cmd2.SelectCommand.Parameters.Add(CustParm2);

                ds = new WWTToursDataset();

                Cmd2.Fill(ds, ds.Tables[0].TableName);
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


        }



        public WWTToursDataset dsWWTTourData
        { get { return ds; } }
        public string ErrorMsg
        { get { return strErrorMsg; } }
    }
   
    }





