using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Xml;
using WWTWebservices;

namespace WWT.Providers
{
    public abstract partial class GetTours : RequestProvider
    {
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
                            xmlWriter.WriteAttributeString("AverageRating", tr.AverageRating.ToString());
                            xmlWriter.WriteAttributeString("LengthInSecs", tr.LengthInSecs.ToString());
                            xmlWriter.WriteAttributeString("OrganizationUrl", tr.OrganizationURL);
                            xmlWriter.WriteAttributeString("OrganizationName", tr.OrganizationName);
                            xmlWriter.WriteAttributeString("ITHList", tr.TourITHList);
                            xmlWriter.WriteAttributeString("AstroObjectsList", tr.TourAstroObjectList);
                            xmlWriter.WriteAttributeString("Keywords", tr.TourKeywordList);
                            xmlWriter.WriteAttributeString("RelatedTours", tr.TourExplicitTourLinkList);
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

        internal static SqlConnection GetConnectionWWTTours()
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

                    OrdCol = MySQLReader.GetOrdinal("AverageRating");
                    if (MySQLReader[OrdCol] != DBNull.Value)
                    {
                        averageRating = MySQLReader.GetDouble(OrdCol);
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
                    loadTour.TourITHList = tourithlist;
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
                            xmlWriter.WriteAttributeString("AverageRating", tr.AverageRating.ToString());
                            xmlWriter.WriteAttributeString("LengthInSecs", tr.LengthInSecs.ToString());
                            xmlWriter.WriteAttributeString("OrganizationUrl", tr.OrganizationURL);
                            xmlWriter.WriteAttributeString("OrganizationName", tr.OrganizationName);
                            xmlWriter.WriteAttributeString("ITHList", tr.TourITHList);
                            xmlWriter.WriteAttributeString("AstroObjectsList", tr.TourAstroObjectList);
                            xmlWriter.WriteAttributeString("Keywords", tr.TourKeywordList);
                            xmlWriter.WriteAttributeString("RelatedTours", tr.TourExplicitTourLinkList);
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

        int GetSQLTourArrayList(List<Tour> SQLTours, string query)
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
                tr.TourITHList = Convert.ToString(dr["TourITHList"]);
                tr.TourAstroObjectList = Convert.ToString(dr["TourAstroObjectList"]);
                tr.TourKeywordList = Convert.ToString(dr["TourKeywordList"]);
                tr.TourExplicitTourLinkList = Convert.ToString(dr["TourExplicitTourLinkList"]);
                SQLTours.Add(tr);
            }

            return 0;
        }

        private void AddToursToChildNode(XmlWriter xmlWriter, int parcatId)
        {
            List<Tour> SQLTours = new List<Tour>();
            int nRet = GetSQLTourArrayList(SQLTours, "exec spGetWWTToursForDateRangeFromCatId " + parcatId + ", null, null, 0");

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
                xmlWriter.WriteAttributeString("AverageRating", tr.AverageRating.ToString());
                xmlWriter.WriteAttributeString("LengthInSecs", tr.LengthInSecs.ToString());
                xmlWriter.WriteAttributeString("OrganizationUrl", tr.OrganizationURL);
                xmlWriter.WriteAttributeString("OrganizationName", tr.OrganizationName);
                xmlWriter.WriteAttributeString("ITHList", tr.TourITHList);
                xmlWriter.WriteAttributeString("AstroObjectsList", tr.TourAstroObjectList);
                xmlWriter.WriteAttributeString("Keywords", tr.TourKeywordList);
                xmlWriter.WriteAttributeString("RelatedTours", tr.TourExplicitTourLinkList);
                xmlWriter.WriteEndElement();
            }

            StoredProc sp1 = new StoredProc("exec spGetSubCatDetailsFromParCatId " + parcatId.ToString());
            DataTable dt = new DataTable();
            int nRet1 = sp1.RunQuery(dt);
            sp1.Dispose();

            foreach (DataRow dr in dt.Rows)
            {
                int tempcatId = Convert.ToInt32(dr[0]);
                int tempparcatId = Convert.ToInt32(dr[1]);
                string catName = Convert.ToString(dr[2]);
                string catTNUrl = Convert.ToString(dr[3]);

                xmlWriter.WriteStartElement("Folder");
                xmlWriter.WriteAttributeString("Name", catName);
                xmlWriter.WriteAttributeString("Group", "Tour");
                xmlWriter.WriteAttributeString("Thumbnail", catTNUrl);
                AddToursToChildNode(xmlWriter, tempcatId);
                xmlWriter.WriteEndElement();
            }
        }

        private string LoadTourHierarchy()
        {
            List<Tour> SQLTours = new List<Tour>();
            int nRet = GetSQLTourArrayList(SQLTours, "exec spGetWWTToursForDateRangeFromCatId 0, null, null, 0");

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
                {

                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                    xmlWriter.WriteStartElement("Folder");

                    if (SQLTours.Count > 0)
                    {

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
                            xmlWriter.WriteAttributeString("AverageRating", tr.AverageRating.ToString());
                            xmlWriter.WriteAttributeString("LengthInSecs", tr.LengthInSecs.ToString());
                            xmlWriter.WriteAttributeString("OrganizationUrl", tr.OrganizationURL);
                            xmlWriter.WriteAttributeString("OrganizationName", tr.OrganizationName);
                            xmlWriter.WriteAttributeString("ITHList", tr.TourITHList);
                            xmlWriter.WriteAttributeString("AstroObjectsList", tr.TourAstroObjectList);
                            xmlWriter.WriteAttributeString("Keywords", tr.TourKeywordList);
                            xmlWriter.WriteAttributeString("RelatedTours", tr.TourExplicitTourLinkList);
                            xmlWriter.WriteEndElement();
                        }
                    }

                    StoredProc sp1 = new StoredProc("exec spGetSubCatDetailsForRootCat");
                    DataTable dt = new DataTable();
                    int nRet1 = sp1.RunQuery(dt);
                    sp1.Dispose();

                    foreach (DataRow dr in dt.Rows)
                    {
                        int catId = Convert.ToInt32(dr[0]);
                        int parcatId = Convert.ToInt32(dr[1]);
                        string catName = Convert.ToString(dr[2]);
                        string catTNUrl = Convert.ToString(dr[3]);

                        xmlWriter.WriteStartElement("Folder");
                        xmlWriter.WriteAttributeString("Name", catName);
                        xmlWriter.WriteAttributeString("Group", "Tour");
                        xmlWriter.WriteAttributeString("Thumbnail", catTNUrl);
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
            int fromSQLVersion;
            int MinutesToAdd;

            List<Tour> SQLTours = new List<Tour>();

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
                    needToBuild = true;

                    if (fromSQLVersion != fromCacheVersion)
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
                fromSQLVersion = GetSQLToursVersion();
                HttpContext.Current.Cache.Remove("Version");
                HttpContext.Current.Cache.Add("Version", fromSQLVersion, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

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
            int fromSQLVersion;
            int MinutesToAdd;

            List<Tour> SQLTours = new List<Tour>();

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
}
