using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using WWTWebservices;

namespace WWTMVC5.WWTWeb
{
    

    public partial class GetTourList : System.Web.UI.Page
    {
        
        public static string GetToursXML()
        {
            List<Tour> sqlTours = new List<Tour>();
            int rc = GetSQLTourArrayList(sqlTours);

            if (sqlTours.Count > 0)
            {
                using (StringWriter sw = new StringWriter())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                        xmlWriter.WriteStartElement("Folder");
                        foreach (Tour tr in sqlTours)
                        {
                            xmlWriter.WriteStartElement("Tour");
                            xmlWriter.WriteAttributeString("Title", tr.TourTitle);
                            xmlWriter.WriteAttributeString("ID", tr.TourGuid.ToString());
                            xmlWriter.WriteAttributeString("Description", tr.TourDescription);
                            xmlWriter.WriteAttributeString("Classification", "Other");
                            xmlWriter.WriteAttributeString("AuthorEmail", tr.AuthorEmailAddress);
                            xmlWriter.WriteAttributeString("Author", tr.AuthorName);
                            xmlWriter.WriteAttributeString("AuthorUrl", tr.AuthorURL);
                            xmlWriter.WriteAttributeString("AverageRating", tr.AverageRating.ToString());
                            xmlWriter.WriteAttributeString("LengthInSecs", tr.LengthInSecs.ToString());
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

        internal static SqlConnection GetConnectionWwtTours()
        {
            string connStr = null;
            connStr = ConfigurationManager.AppSettings["WWTToursDBConnectionString"];
            SqlConnection myConnection = null;
            myConnection = new SqlConnection(connStr);
            return myConnection;
        }

        public static int GetSQLTourArrayList(List<Tour> sqlTours)
        {
            string strErrorMsg;
            //int version = -1;
            DateTime dtBeginDateTime;
            DateTime dtEndDateTime;
            int ordCol;
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

            strErrorMsg = "";
            SqlConnection myConnection5 = GetConnectionWwtTours();

            dtBeginDateTime = Convert.ToDateTime("1/1/1900");
            dtEndDateTime = Convert.ToDateTime("1/1/2100");

            try
            {
                myConnection5.Open();

                SqlCommand cmd = null;
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 20;
                cmd.Connection = myConnection5;

                cmd.CommandText = "spGetWWTToursForDateRange";

                SqlParameter custParm = new SqlParameter("@pBeginDateTime", SqlDbType.DateTime);
                custParm.Value = dtBeginDateTime;
                cmd.Parameters.Add(custParm);

                SqlParameter custParm2 = new SqlParameter("@pEndDateTime", SqlDbType.DateTime);
                custParm2.Value = dtEndDateTime;
                cmd.Parameters.Add(custParm2);

                System.Data.SqlClient.SqlDataReader mySqlReader;
                mySqlReader = cmd.ExecuteReader();
                while (mySqlReader.Read())
                {
                    ordCol = mySqlReader.GetOrdinal("TourGUID");
                    tourguid = mySqlReader.GetGuid(ordCol);
                    ordCol = mySqlReader.GetOrdinal("TourTitle");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourtitle = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        tourtitle = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("WorkFlowStatusCode");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        workflowstatuscode = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        workflowstatuscode = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourSubmittedDateTime");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        toursubmitteddatetime = Convert.ToDateTime(mySqlReader.GetSqlDateTime(ordCol).ToString());
                    }
                    else
                    {
                        toursubmitteddatetime = DateTime.MinValue;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourApprovedDateTime");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourapproveddatetime = Convert.ToDateTime(mySqlReader.GetSqlDateTime(ordCol).ToString());
                    }
                    else
                    {
                        tourapproveddatetime = DateTime.MinValue;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourRejectedDateTime");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourrejecteddatetime = Convert.ToDateTime(mySqlReader.GetSqlDateTime(ordCol).ToString());
                    }
                    else
                    {
                        tourrejecteddatetime = DateTime.MinValue;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourApprovedRejectedByName");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourapprovedrejectedbyname = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        tourapprovedrejectedbyname = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourDescription");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourdescription = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        tourdescription = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourAttributionAndCredits");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourattributionandcredits = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        tourattributionandcredits = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("AuthorName");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        authorname = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        authorname = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("AuthorEmailAddress");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        authoremailaddress = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        authoremailaddress = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("AuthorURL");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        authorurl = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        authorurl = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("AuthorSecondaryEmailAddress");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        authorsecondaryemailaddress = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        authorsecondaryemailaddress = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("AuthorContactPhoneNumber");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        authorcontactphonenumber = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        authorcontactphonenumber = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("AuthorContactText");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        authorcontacttext = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        authorcontacttext = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("OrganizationName");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        organizationname = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        organizationname = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("OrganizationURL");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        organizationurl = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        organizationurl = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourKeywordList");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourkeywordlist = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        tourkeywordlist = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourITHList");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourithlist = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        tourithlist = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourAstroObjectList");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourastroobjectlist = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        tourastroobjectlist = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("TourExplicitTourLinkList");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        tourexplicittourlinklist = mySqlReader.GetString(ordCol);
                    }
                    else
                    {
                        tourexplicittourlinklist = null;
                    }

                    ordCol = mySqlReader.GetOrdinal("LengthInSecs");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        lengthinsecs = mySqlReader.GetInt32(ordCol);
                    }
                    else
                    {
                        lengthinsecs = -1;
                    }

                    ordCol = mySqlReader.GetOrdinal("AverageRating");
                    if (mySqlReader[ordCol] != DBNull.Value)
                    {
                        averageRating = mySqlReader.GetDouble(ordCol);
                    }
                    else
                    {
                        averageRating = 0;
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
                    loadTour.OrganizationURL = organizationurl;
                    loadTour.TourKeywordList = tourkeywordlist;
                    //loadTour.TourITHList - tourithlist;
                    loadTour.TourAstroObjectList = tourastroobjectlist;
                    loadTour.TourExplicitTourLinkList = tourexplicittourlinklist;
                    loadTour.LengthInSecs = lengthinsecs;
                    loadTour.AverageRating = averageRating;
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

        public static int GetSqlToursVersion()
        {
            string strErrorMsg;
            int version = -1;

            strErrorMsg = "";
            SqlConnection myConnection5 = GetConnectionWwtTours();

            try
            {
                myConnection5.Open();

                SqlCommand cmd = null;
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 20;
                cmd.Connection = myConnection5;

                cmd.CommandText = "spGetTourVersion";

                System.Data.SqlClient.SqlDataReader mySqlReader;
                mySqlReader = cmd.ExecuteReader();
                mySqlReader.Read();
                int ordVersionNumber = mySqlReader.GetOrdinal("VersionNumber");
                version = mySqlReader.GetInt32(ordVersionNumber);

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

        public static string GetToursXML(List<Tour> sqlTours)
        {
            if (sqlTours.Count > 0)
            {
                using (StringWriter sw = new StringWriter())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                        xmlWriter.WriteStartElement("Folder");
                        foreach (Tour tr in sqlTours)
                        {
                            xmlWriter.WriteStartElement("Tour");
                            xmlWriter.WriteAttributeString("Title", tr.TourTitle);
                            xmlWriter.WriteAttributeString("ID", tr.TourGuid.ToString());
                            xmlWriter.WriteAttributeString("Description", tr.TourDescription);
                            xmlWriter.WriteAttributeString("Classification", "Other");
                            xmlWriter.WriteAttributeString("AuthorEmail", tr.AuthorEmailAddress);
                            xmlWriter.WriteAttributeString("Author", tr.AuthorName);
                            xmlWriter.WriteAttributeString("AuthorUrl", tr.AuthorURL);
                            xmlWriter.WriteAttributeString("AverageRating", tr.AverageRating.ToString());
                            xmlWriter.WriteAttributeString("LengthInSecs", tr.LengthInSecs.ToString());
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

        int GetSQLTourArrayList(List<Tour> sqlTours, string query)
        {
            StoredProc sp = new StoredProc(query);
            DataTable dt = new DataTable();
            int nRet = sp.RunQuery(dt);
            sp.Dispose();
            foreach (DataRow dr in dt.Rows)
            {
                Tour tr = new Tour();
                tr.TourTitle = Convert.ToString(dr["TourTitle"]);
                tr.TourGuid = new Guid(dr["TourGUID"].ToString());
                tr.TourDescription = Convert.ToString(dr["TourDescription"]);
                tr.AuthorEmailAddress = Convert.ToString(dr["AuthorEmailAddress"]);
                tr.AuthorName = Convert.ToString(dr["AuthorName"]);
                tr.AuthorURL = Convert.ToString(dr["AuthorURL"]);
                tr.AverageRating = Convert.ToDouble(dr["AverageRating"]);
                tr.LengthInSecs = Convert.ToInt32(dr["LengthInSecs"]);
                tr.OrganizationURL = Convert.ToString(dr["OrganizationURL"]);
                tr.OrganizationName = Convert.ToString(dr["OrganizationName"]);
                sqlTours.Add(tr);
            }

            return 0;
        }

        private void AddToursToChildNode(XmlWriter xmlWriter, int parcatId)
        {
            List<Tour> sqlTours = new List<Tour>();
            int nRet = GetSQLTourArrayList(sqlTours, "exec spGetWWTToursForDateRangeFromCatId " + parcatId + ", null, null, 0");

            foreach (Tour tr in sqlTours)
            {
                xmlWriter.WriteStartElement("Tour");
                xmlWriter.WriteAttributeString("Title", tr.TourTitle);
                xmlWriter.WriteAttributeString("ID", tr.TourGuid.ToString());
                xmlWriter.WriteAttributeString("Description", tr.TourDescription);
                xmlWriter.WriteAttributeString("Classification", "Other");
                xmlWriter.WriteAttributeString("AuthorEmail", tr.AuthorEmailAddress);
                xmlWriter.WriteAttributeString("Author", tr.AuthorName);
                xmlWriter.WriteAttributeString("AuthorUrl", tr.AuthorURL);
                xmlWriter.WriteAttributeString("AverageRating", tr.AverageRating.ToString());
                xmlWriter.WriteAttributeString("LengthInSecs", tr.LengthInSecs.ToString());
                xmlWriter.WriteAttributeString("OrganizationUrl", tr.OrganizationURL);
                xmlWriter.WriteAttributeString("OrganizationName", tr.OrganizationName);
                xmlWriter.WriteEndElement();
            }

            StoredProc sp1 = new StoredProc("Select CategoryId, ParentCatID, Name, CatThumbnailUrl from TourCategories where ParentCatId = " + parcatId.ToString() + " and CategoryId <> " + parcatId);
            DataTable dt = new DataTable();
            int nRet1 = sp1.RunQuery(dt);
            sp1.Dispose();

            foreach (DataRow dr in dt.Rows)
            {
                int tempcatId = Convert.ToInt32(dr[0]);
                int tempparcatId = Convert.ToInt32(dr[1]);
                string catName = Convert.ToString(dr[2]);
                string catTnUrl = Convert.ToString(dr[3]);

                xmlWriter.WriteStartElement("Folder");
                xmlWriter.WriteAttributeString("Name", catName);
                xmlWriter.WriteAttributeString("Group", "Tour");
                xmlWriter.WriteAttributeString("Thumbnail", catTnUrl);
                AddToursToChildNode(xmlWriter, tempcatId);
                xmlWriter.WriteEndElement();
            }
        }

        private string LoadTourHierarchy()
        {
            List<Tour> sqlTours = new List<Tour>();
            int nRet = GetSQLTourArrayList(sqlTours, "exec spGetWWTToursForDateRangeFromCatId 0, null, null, 0");

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
                {

                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                    xmlWriter.WriteStartElement("Folder");

                    if (sqlTours.Count > 0)
                    {

                        foreach (Tour tr in sqlTours)
                        {
                            xmlWriter.WriteStartElement("Tour");
                            xmlWriter.WriteAttributeString("Title", tr.TourTitle);
                            xmlWriter.WriteAttributeString("ID", tr.TourGuid.ToString());
                            xmlWriter.WriteAttributeString("Description", tr.TourDescription);
                            xmlWriter.WriteAttributeString("Classification", "Other");
                            xmlWriter.WriteAttributeString("AuthorEmail", tr.AuthorEmailAddress);
                            xmlWriter.WriteAttributeString("Author", tr.AuthorName);
                            xmlWriter.WriteAttributeString("AuthorUrl", tr.AuthorURL);
                            xmlWriter.WriteAttributeString("AverageRating", tr.AverageRating.ToString());
                            xmlWriter.WriteAttributeString("LengthInSecs", tr.LengthInSecs.ToString());
                            xmlWriter.WriteAttributeString("OrganizationUrl", tr.OrganizationURL);
                            xmlWriter.WriteAttributeString("OrganizationName", tr.OrganizationName);
                            xmlWriter.WriteEndElement();
                        }
                    }

                    StoredProc sp1 = new StoredProc("Select CategoryId, ParentCatID, Name, CatThumbnailUrl from TourCategories where ParentCatId = 0 and CategoryId <> 0");
                    DataTable dt = new DataTable();
                    int nRet1 = sp1.RunQuery(dt);
                    sp1.Dispose();

                    foreach (DataRow dr in dt.Rows)
                    {
                        int catId = Convert.ToInt32(dr[0]);
                        int parcatId = Convert.ToInt32(dr[1]);
                        string catName = Convert.ToString(dr[2]);
                        string catTnUrl = Convert.ToString(dr[3]);

                        xmlWriter.WriteStartElement("Folder");
                        xmlWriter.WriteAttributeString("Name", catName);
                        xmlWriter.WriteAttributeString("Group", "Tour");
                        xmlWriter.WriteAttributeString("Thumbnail", catTnUrl);
                        AddToursToChildNode(xmlWriter, catId);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                    xmlWriter.Close();
                }
                sw.Close();
                return sw.ToString();
                //StreamWriter fsw = new StreamWriter(@"c:\\test_Tour.xml");
                //fsw.WriteLine(sw.ToString());
                //fsw.Close();
            }
        }

        public int UpdateCacheEx()
        {
            bool needToBuild;
            int fromCacheVersion;
            int fromSqlVersion;
            int minutesToAdd;

            List<Tour> sqlTours = new List<Tour>();

            needToBuild = false;

            DateTime fromCacheDateTime;

            if (HttpContext.Current.Cache.Get("WWTXMLTours") == null)
            {
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
                minutesToAdd = Int32.Parse(ConfigurationManager.AppSettings["TourVersionCheckIntervalMinutes"]);
            }
            catch
            {
                minutesToAdd = 5;  // if missing config, set to 5 minutes
            }

            if (System.DateTime.Now > fromCacheDateTime.AddMinutes(minutesToAdd))
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
                    fromSqlVersion = GetSqlToursVersion();

                    if (fromSqlVersion != fromCacheVersion)
                    {
                        needToBuild = true;
                    }
                }

            }

            if (needToBuild)
            {

                HttpContext.Current.Cache.Remove("LastCacheUpdateDateTime");
                HttpContext.Current.Cache.Add("LastCacheUpdateDateTime", System.DateTime.Now, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

                //update the version number in the cache (datetime is already updated)
                fromSqlVersion = GetSqlToursVersion();
                HttpContext.Current.Cache.Remove("Version");
                HttpContext.Current.Cache.Add("Version", fromSqlVersion, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

                //update the WWTTours cache with the SQLTours ArrayList
                HttpContext.Current.Cache.Remove("WWTXMLTours");
                HttpContext.Current.Cache.Add("WWTXMLTours", LoadTourHierarchy(), null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            }
            return 0;
        }

        public static int UpdateCache()
        {
            bool needToBuild;
            int fromCacheVersion;
            int fromSqlVersion;
            int minutesToAdd;

            List<Tour> sqlTours = new List<Tour>();

            needToBuild = false;

            DateTime fromCacheDateTime;

            if (HttpContext.Current.Cache.Get("WWTXMLTours") == null)
            {
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
                minutesToAdd = Int32.Parse(ConfigurationManager.AppSettings["TourVersionCheckIntervalMinutes"]);
            }
            catch
            {
                minutesToAdd = 5;  // if missing config, set to 5 minutes
            }

            if (System.DateTime.Now > fromCacheDateTime.AddMinutes(minutesToAdd))
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
                    fromSqlVersion = GetSqlToursVersion();

                    if (fromSqlVersion != fromCacheVersion)
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

                int rc = GetSQLTourArrayList(sqlTours);

                //update the version number in the cache (datetime is already updated)
                fromSqlVersion = GetSqlToursVersion();
                HttpContext.Current.Cache.Remove("Version");
                HttpContext.Current.Cache.Add("Version", fromSqlVersion, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

                //update the WWTTours cache with the SQLTours ArrayList
                HttpContext.Current.Cache.Remove("WWTXMLTours");
                HttpContext.Current.Cache.Add("WWTXMLTours", GetToursXML(sqlTours), null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            }
            return 0;
        }
    }
}