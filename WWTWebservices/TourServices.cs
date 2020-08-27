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
/// TourServices : 
///     GetTourGUIDFromXML
///     GetToursUNC
///     UpdateCache
///     GetCache
///     GetSQLTourArrayList
///     GetSQLToursVersion
///     WWTTourForAuthor
///     WWTTourForKeyword
///     WWTTourForDateRange
///     WWTTourForGUID
///     WWTTour
///     WWTTourUpdtStatus
/// 
/// </summary>
/// 
namespace WWTWebservices
{
    internal static class TourServices
    {
        public static string GetTourGUIDFromXML(string TourXML)
        {
            string TourGUIDString = "failname";

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(TourXML);
            }
            catch (Exception)
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


        public static int UpdateCache()
        {
            bool needToBuild;
            int fromCacheVersion;
            int fromSQLVersion;
            int MinutesToAdd;

            List<Tour> SQLTours = new List<Tour>();

            needToBuild = false;

            DateTime fromCacheDateTime;
            DateTime lastFlushDateTime;

            if (HttpContext.Current.Cache.Get("WWTTours") == null)
            {
                needToBuild = true;
            }
            // see if you need to build the cache.... 

            // first : completely reload cache (esp to get updated ratings) even if no changes every N minutes
            try
            {
                lastFlushDateTime = (DateTime)HttpContext.Current.Cache.Get("LastFlushDateTime");
            }
            catch
            {
                lastFlushDateTime = System.DateTime.Now.AddDays(-1);
            }

            try
            {
                MinutesToAdd = Int32.Parse(ConfigurationManager.AppSettings["CompleteFlushIntervalMinutes"]);
            }
            catch
            {
                MinutesToAdd = 60;  // if missing config, set to 60 minutes
            }

            if (System.DateTime.Now > lastFlushDateTime.AddMinutes(MinutesToAdd))
            {
                needToBuild = true;
                // remove cache for WWTTours; LastUpdateDateTime; Version Number
                HttpContext.Current.Cache.Remove("LastCacheUpdateDateTime");
                HttpContext.Current.Cache.Remove("Version");
                HttpContext.Current.Cache.Remove("WWTTours");

                HttpContext.Current.Cache.Add("LastFlushDateTime", System.DateTime.Now, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

                DateTime xxx = (DateTime)HttpContext.Current.Cache.Get("LastFlushDateTime");

            }
           

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
                HttpContext.Current.Cache.Remove("WWTTours");
                HttpContext.Current.Cache.Add("WWTTours", SQLTours, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

            }
            return 0;
        }


        public static List<Tour> GetCache()
        {
            List<Tour> myTours = new List<Tour>();

            //myTours.Add(new Tour()); 

            //Tour  fromCache  = new Tour();
            myTours = (List<Tour>)HttpContext.Current.Cache.Get("WWTTours");
            if (myTours.Count == 0)
            {
                HttpContext.Current.Cache.Remove("WWTTours");
                int x = UpdateCache();
                myTours = (List<Tour>)HttpContext.Current.Cache.Get("WWTTours");
            }

            return myTours;
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
            double averageRating;
            int numberOfRatings;
            int numberOfObjections;



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

                    OrdCol = MySQLReader.GetOrdinal("AverageRating");
                    if (MySQLReader[OrdCol] != DBNull.Value)
                    {
                        averageRating = MySQLReader.GetDouble(OrdCol);
                    }
                    else
                    {
                        averageRating = -1;
                    }

                    OrdCol = MySQLReader.GetOrdinal("NumberOfRatings");
                    if (MySQLReader[OrdCol] != DBNull.Value)
                    {
                        numberOfRatings = MySQLReader.GetInt32(OrdCol);
                    }
                    else
                    {
                        numberOfRatings = -1;
                    }

                    OrdCol = MySQLReader.GetOrdinal("NumberOfObjections");
                    if (MySQLReader[OrdCol] != DBNull.Value)
                    {
                        numberOfObjections = MySQLReader.GetInt32(OrdCol);
                    }
                    else
                    {
                        numberOfObjections = -1;
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
                    loadTour.AverageRating = averageRating;
                    //loadTour.NumberOfRatings = numberOfRatings;
                    //loadTour.NumberOfObjections = numberOfObjections;
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

        public class WWTTourForAuthor
        {
            private WWTToursDataset ds;
            private string strErrorMsg;
            private List<Tour> ds2;

            public WWTTourForAuthor(string AuthorName)
            {
                Tour cacheTour;
                int AuthorFoundPtr = -1;

                int x = UpdateCache();

                List<Tour> whatiscache = GetCache();

                List<Tour> SelectedTours = new List<Tour>();

                // loop thru cache - find the matching keywords
                for (int c = 0; c < whatiscache.Count; c++)
                {
                    cacheTour = new Tour();
                    cacheTour = (Tour)whatiscache[c];
                    string myAuthorName = cacheTour.AuthorName;
                    if (myAuthorName == AuthorName)  //(myGuidStr == TourGUID)
                    {
                        AuthorFoundPtr = c;
                        SelectedTours.Add(cacheTour);
                    }
                }
                if (AuthorFoundPtr > -1)
                {
                    ds2 = SelectedTours;
                }
                else
                { 

                strErrorMsg = "";
                SqlConnection myConnection5 = Database.GetConnectionWWTTours();

                try
                {

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
                    throw
                        WWTWebService.RaiseException("GetToursForAuthor", "http://WWTWebServices", ex.Message, "2000", "GetToursForAuthor", WWTWebService.FaultCode.Client);
                }
                finally
                {
                    if (myConnection5.State == ConnectionState.Open)
                        myConnection5.Close();
                }
            }


            }

            public List<Tour> dsTourFromCache
            { get { return ds2; } }
            public WWTToursDataset dsWWTTourData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }


        public class WWTTourForKeyword
        {
            private WWTToursDataset ds;
            private string strErrorMsg;
            private List<Tour> ds2;

            public WWTTourForKeyword(string Keyword)
            {
                Tour cacheTour;
                int TourFoundPtr = -1;

                int x = UpdateCache();

                List<Tour> whatiscache = GetCache();

                List<Tour> SelectedTours = new List<Tour>();

                // loop thru cache - find the matching keywords
                for (int c = 0; c < whatiscache.Count; c++)
                {
                    cacheTour = new Tour();
                    cacheTour = (Tour)whatiscache[c];
                    string myKeywords = cacheTour.TourKeywordList;
                    char[] delimiterChars = { ' ' };
                    if (myKeywords != null)
                    {
                        string[] singleWord = myKeywords.Split(delimiterChars);
                        foreach (string s in singleWord)
                        {
                            if (s == Keyword)
                            {
                                if (TourFoundPtr != c)
                                {
                                    TourFoundPtr = c;
                                    SelectedTours.Add(cacheTour);
                                }
                            }
                        }
                    }
                }
                if (TourFoundPtr > -1)
                {
                    ds2 = SelectedTours;
                }
                else
                {

                strErrorMsg = "";
                SqlConnection myConnection5 = Database.GetConnectionWWTTours();

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
                    throw
                        WWTWebService.RaiseException("GetToursForKeyword", "http://WWTWebServices", ex.Message, "2000", "GetToursForKeyword", WWTWebService.FaultCode.Client);
                }
                finally
                {
                    if (myConnection5.State == ConnectionState.Open)
                        myConnection5.Close();
                }
            }


            }


            public List<Tour> dsTourFromCache
            { get { return ds2; } }
            public WWTToursDataset dsWWTTourData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }

        public class WWTTourForDateRange
        {
            private WWTToursDataset ds;
            private string strErrorMsg;
            private List<Tour> ds2;

            public WWTTourForDateRange(string BeginDateTime, string EndDateTime)
            {
                Tour cacheTour;
                int TourFoundPtr = -1;

                int x = UpdateCache();

                List<Tour> whatiscache = GetCache();

                List<Tour> SelectedTours = new List<Tour>();

                DateTime dtBeginDateTime;
                DateTime dtEndDateTime;

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

                // loop thru cache - find the matching keywords
                for (int c = 0; c < whatiscache.Count; c++)
                {
                    cacheTour = new Tour();
                    cacheTour = (Tour)whatiscache[c];
                    DateTime myDateTime = cacheTour.TourSubmittedDateTime;
                    if (myDateTime >= dtBeginDateTime & myDateTime <= dtEndDateTime)  //(myGuidStr == TourGUID)
                    {
                        TourFoundPtr = c;
                        SelectedTours.Add(cacheTour);
                    }
                }
                if (TourFoundPtr > -1)
                {
                    ds2 = SelectedTours;
                }
                else
                {
                    strErrorMsg = "";

                    SqlConnection myConnection5 = Database.GetConnectionWWTTours();


                    try
                    {

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
                        throw
                            WWTWebService.RaiseException("GetToursForDateRange", "http://WWTWebServices", ex.Message, "2000", "GetToursForDateRange", WWTWebService.FaultCode.Client);
                    }
                    finally
                    {
                        if (myConnection5.State == ConnectionState.Open)
                            myConnection5.Close();
                    }

                }

            }

            public List<Tour> dsTourFromCache
            { get { return ds2; } }
            public WWTToursDataset dsWWTTourData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }

        public class WWTTourForGUID
        {
            private WWTToursDataset ds;
            private List<Tour> ds2;
            private string strErrorMsg;

            public WWTTourForGUID(string TourGUID)
            {
                strErrorMsg = "";
                SqlConnection myConnection5 = Database.GetConnectionWWTTours();

                Tour cacheTour;
                int GuidFoundPtr = -1;

                try
                {
                    int x = UpdateCache();

                    List<Tour> whatiscache = GetCache();

                    List<Tour> SelectedTours = new List<Tour>();

                    // loop thru cache - find the GUID
                    for (int c = 0; c < whatiscache.Count; c++)
                    {
                        cacheTour = new Tour();
                        cacheTour = (Tour)whatiscache[c];
                        string myGuidStr = cacheTour.TourGuid.ToString();
                        if (myGuidStr == TourGUID)
                        {
                            GuidFoundPtr = c;
                            SelectedTours.Add(cacheTour);
                        }
                    }
                    if (GuidFoundPtr > -1)
                    {
                        ds2 = SelectedTours;
                    }
                    else
                    {
                        myConnection5.Open();
                        SqlDataAdapter Cmd2 = new SqlDataAdapter("spGetWWTToursForGUID", myConnection5);

                        Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                        SqlParameter CustParm = new SqlParameter("@pWWTTourGUID", SqlDbType.VarChar);
                        CustParm.Value = TourGUID;
                        Cmd2.SelectCommand.Parameters.Add(CustParm);

                        ds = new WWTToursDataset();

                        Cmd2.Fill(ds, ds.Tables[0].TableName);

                    }

                    //cacheTour = new Tour();
                    //cacheTour = (Tour)whatiscache[0];
                    //string myName = cacheTour.AuthorName;

                }
                catch (Exception ex)
                {
                    throw
                        WWTWebService.RaiseException("WWTTourForGUID", "http://WWTWebServices", ex.Message, "2000", "WWTTourForGUID", WWTWebService.FaultCode.Client);
                }
                finally
                {
                    if (myConnection5.State == ConnectionState.Open)
                        myConnection5.Close();
                }


            }

            public List<Tour> dsTourFromCache
            { get { return ds2; } }
            public WWTToursDataset dsWWTTourData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }



        public class WWTTour
        {
            private WWTToursDataset ds;
            private List<Tour> ds2;
            private string strErrorMsg;


            Tour cacheTour;
            string TourGUIDString = "";

            Guid outTourGuid;
            string outTourGuidString;
            string outTourTitle;



            public WWTTour(string TourXML, byte[] TourBlob, byte[] TourThumbnail, byte[] AuthorThumbnail)
          {
                strErrorMsg = "";
                SqlConnection myConnection5 = Database.GetConnectionWWTTours();


                // cut file stuff from here

                try
                {
                    //
                    SqlCommand Cmd5 = null;

                    Cmd5 = new SqlCommand();
                    Cmd5.Connection = myConnection5;
                    Cmd5.CommandType = CommandType.StoredProcedure;
                    Cmd5.CommandTimeout = 10000;

                    SqlParameter CustParm = Cmd5.CreateParameter();
                    CustParm.SqlDbType = SqlDbType.NVarChar;
                    CustParm.ParameterName = "@pInputXMLStream";
                    Cmd5.Parameters.Add(CustParm);
                    CustParm.Value = TourXML;

                    Cmd5.CommandText = "spImportTour";
                    myConnection5.Open();

                    ds = new WWTToursDataset();
                    System.Data.SqlClient.SqlDataReader MySQLReader;

                    MySQLReader = Cmd5.ExecuteReader();


                    while (MySQLReader.Read())
                    {
                        int OrdTourGUID = MySQLReader.GetOrdinal("TourGUID");
                        outTourGuid = MySQLReader.GetGuid(OrdTourGUID);
                        outTourGuidString = outTourGuid.ToString();

                        int OrdTourTitle = MySQLReader.GetOrdinal("TourTitle");
                        outTourTitle = MySQLReader.GetString(OrdTourTitle);
                   }

                    
                    List<Tour> SelectedTours = new List<Tour>();

                        cacheTour = new Tour();
                        cacheTour.TourGuid = outTourGuid;
                        cacheTour.TourTitle = outTourTitle;
                        cacheTour.WorkFlowStatusCode = "0";
                        cacheTour.TourSubmittedDateTime = System.DateTime.Now;

                        SelectedTours.Add(cacheTour);
                        ds2 = SelectedTours;

                }
                catch (Exception ex)
                {
                    throw
                        WWTWebService.RaiseException("ImportTour", "http://WWTWebServices", ex.Message, "2000", "ImportTour", WWTWebService.FaultCode.Client);
                }
                finally
                {
                    if (myConnection5.State == ConnectionState.Open)
                        myConnection5.Close();
                }

                try
                {
                    // Tour Thumbnail

                    string strTourUNC = GetToursUNC();
                    TourGUIDString = outTourGuidString;  // WAS :  GetTourGUIDFromXML(TourXML);
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
                    throw
                        WWTWebService.RaiseException("ImportTour", "http://WWTWebServices", ex.Message, "1000", "ImportTour", WWTWebService.FaultCode.Client);
                }

            }



            public List<Tour> dsTourFromCache
            { get { return ds2; } }
            public WWTToursDataset dsWWTTourData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }


        public class WWTTourUpdtStatus
        {
            private WWTToursDataset ds;
            private List<Tour> ds2;
            private string strErrorMsg;

            Tour cacheTour;
            int GuidFoundPtr = -1;

            public WWTTourUpdtStatus(string TourGUID, char WorkFlowStatusCode, string ApprovedRejectedByName)
            {
                strErrorMsg = "";
                SqlConnection myConnection5 = Database.GetConnectionWWTTours();

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

                    int x = UpdateCache();

                    List<Tour> whatiscache = GetCache();

                    List<Tour> SelectedTours = new List<Tour>();

                    // loop thru cache - find the GUID
                    for (int c = 0; c < whatiscache.Count; c++)
                    {
                        cacheTour = new Tour();
                        cacheTour = (Tour)whatiscache[c];
                        string myGuidStr = cacheTour.TourGuid.ToString();
                        if (myGuidStr == TourGUID)
                        {
                            GuidFoundPtr = c;
                            SelectedTours.Add(cacheTour);
                        }
                    }
                    if (GuidFoundPtr > -1)
                    {
                        ds2 = SelectedTours;
                    }
                }
                catch (Exception ex)
                {
                    throw
                        WWTWebService.RaiseException("UpdateTourWorkFlowStatus", "http://WWTWebServices", ex.Message, "2000", "UpdateTourWorkFlowStatus", WWTWebService.FaultCode.Client);
                }
                finally
                {
                    if (myConnection5.State == ConnectionState.Open)
                        myConnection5.Close();
                }

            }

            public List<Tour> dsTourFromCache
            { get { return ds2; } }
            public WWTToursDataset dsWWTTourData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }

 
        public class WWTTourInsertRatingOrComment
        {
            private TourRatingDataset ds;
            private List<Tour> ds2;
            private string strErrorMsg;

            Tour cacheTour;
            int GuidFoundPtr = -1;

            public WWTTourInsertRatingOrComment(string UserGUID, string TourGUID, int Rating, string Comment, int UserSelfRatingID, string UserContactInfo, int ObjectionTypeID, string ObjectionComment)
            {
                strErrorMsg = "";
                SqlConnection myConnection5 = Database.GetConnectionWWTTours();

                try
                {

                    myConnection5.Open();
                    SqlDataAdapter Cmd2 = new SqlDataAdapter("spInsertTourRatingOrComment", myConnection5);

                    Cmd2.SelectCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter CustParm = new SqlParameter("@pUserGUID", SqlDbType.VarChar);
                    CustParm.Value = UserGUID;
                    Cmd2.SelectCommand.Parameters.Add(CustParm);
                    
                    SqlParameter CustParm1 = new SqlParameter("@pTourGUID", SqlDbType.VarChar);
                    CustParm1.Value = TourGUID;
                    Cmd2.SelectCommand.Parameters.Add(CustParm1);

                    SqlParameter CustParm2 = new SqlParameter("@pRating", SqlDbType.Int);
                    if (Rating == -999)
                    {
                        CustParm2.Value = null;
                    }
                    else
                    {
                        CustParm2.Value = Rating;
                    }                    
                    Cmd2.SelectCommand.Parameters.Add(CustParm2);

                    SqlParameter CustParm3 = new SqlParameter("@pComment", SqlDbType.NVarChar);
                    CustParm3.Value = Comment;
                    Cmd2.SelectCommand.Parameters.Add(CustParm3);

                    SqlParameter CustParm4 = new SqlParameter("@pUserSelfRatingID", SqlDbType.Int);
                    if (UserSelfRatingID == -999)
                    {
                        CustParm4.Value = null;
                    }
                    else
                    {
                        CustParm4.Value = UserSelfRatingID;
                    }
                    Cmd2.SelectCommand.Parameters.Add(CustParm4);

                    SqlParameter CustParm5 = new SqlParameter("@pUserContactInfo", SqlDbType.NVarChar);
                    CustParm5.Value = UserContactInfo;
                    Cmd2.SelectCommand.Parameters.Add(CustParm5);

                    SqlParameter CustParm6 = new SqlParameter("@pObjectionTypeID", SqlDbType.Int);
                    if (ObjectionTypeID == -999)
                    {
                        CustParm6.Value = null;
                    }
                    else
                    {
                        CustParm6.Value = ObjectionTypeID;
                    }
                    Cmd2.SelectCommand.Parameters.Add(CustParm6);

                    SqlParameter CustParm7 = new SqlParameter("@pObjectionComment", SqlDbType.NVarChar);
                    CustParm7.Value = ObjectionComment;
                    Cmd2.SelectCommand.Parameters.Add(CustParm7);


                    ds = new TourRatingDataset();

                    Cmd2.Fill(ds, ds.Tables[0].TableName);

                    int x = UpdateCache();

                    List<Tour> whatiscache = GetCache();

                    List<Tour> SelectedTours = new List<Tour>();

                    // loop thru cache - find the GUID
                    for (int c = 0; c < whatiscache.Count; c++)
                    {
                        cacheTour = new Tour();
                        cacheTour = (Tour)whatiscache[c];
                        string myGuidStr = cacheTour.TourGuid.ToString().ToUpper();
                        if (myGuidStr == TourGUID)
                        {
                            GuidFoundPtr = c;
                            SelectedTours.Add(cacheTour);
                        }
                    }
                    if (GuidFoundPtr > -1)
                    {
                        ds2 = SelectedTours;
                    }
                }
                catch (Exception ex)
                {
                    throw
                        WWTWebService.RaiseException("InsertTourRatingOrComment", "http://WWTWebServices", ex.Message, "2000", "InsertTourRatingOrComment", WWTWebService.FaultCode.Client);
                }
                finally
                {
                    if (myConnection5.State == ConnectionState.Open)
                        myConnection5.Close();
                }

            }

            public List<Tour> dsTourFromCache
            { get { return ds2; } }
            public TourRatingDataset dsWWTTourData
            { get { return ds; } }
            public string ErrorMsg
            { get { return strErrorMsg; } }
        }

    }

}
