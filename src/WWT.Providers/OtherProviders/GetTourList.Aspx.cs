#nullable disable

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

using WWT.Tours;

namespace WWT.Providers
{
    public abstract partial class GetTourList : RequestProvider
    {
        private readonly WwtOptions _options;

        public GetTourList(WwtOptions options)
        {
            _options = options;
        }

        protected abstract string SqlCommandString { get; }

        protected abstract string HierarchySqlCommand { get; }

        private int GetSqlToursVersion()
        {
            string strErrorMsg;
            int version = -1;

            strErrorMsg = "";
            SqlConnection myConnection5 = new SqlConnection(_options.WwtToursDBConnectionString);

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

        protected virtual void LoadTourFromRow(DataRow dr, Tour tour)
        {
        }

        private int GetSQLTourArrayList(List<Tour> sqlTours, string query)
        {
            StoredProc sp = new StoredProc(query, _options.WwtToursDBConnectionString);
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

                LoadTourFromRow(dr, tr);

                sqlTours.Add(tr);
            }

            return 0;
        }

        protected virtual void WriteTour(XmlWriter xmlWriter, Tour tr)
        {
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

                WriteTour(xmlWriter, tr);

                xmlWriter.WriteEndElement();
            }

            StoredProc sp1 = new StoredProc(SqlCommandString + parcatId.ToString(), _options.WwtToursDBConnectionString);
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

                            WriteTour(xmlWriter, tr);

                            xmlWriter.WriteEndElement();
                        }
                    }

                    StoredProc sp1 = new StoredProc(HierarchySqlCommand, _options.WwtToursDBConnectionString);
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

        protected int UpdateCacheEx(ICache cache)
        {
            bool needToBuild;
            int fromCacheVersion;
            int fromSqlVersion;
            int minutesToAdd;

            List<Tour> sqlTours = new List<Tour>();

            needToBuild = false;

            DateTime fromCacheDateTime;

            if (cache.Get("WWTXMLTours") == null)
            {
                needToBuild = true;
            }
            // see if you need to build the cache.... 

            // if it has been more than n minutes since you last checked the version, then 
            //  get the version number from sql.   if different, then needtoupdate.  
            try
            {
                fromCacheDateTime = (DateTime)cache.Get("LastCacheUpdateDateTime");
            }
            catch
            {
                fromCacheDateTime = System.DateTime.Now.AddDays(-1);
            }



            minutesToAdd = _options.TourVersionCheckIntervalMinutes;

            if (System.DateTime.Now > fromCacheDateTime.AddMinutes(minutesToAdd))
            {
                try
                {
                    fromCacheVersion = (int)cache.Get("Version");
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
                    needToBuild = true;

                    if (fromSqlVersion != fromCacheVersion)
                    {
                        needToBuild = true;
                    }
                }
            }

            if (needToBuild)
            {
                cache.Remove("LastCacheUpdateDateTime");
                cache.Add("LastCacheUpdateDateTime", System.DateTime.Now, DateTime.MaxValue, new TimeSpan(24, 0, 0));

                //update the version number in the cache (datetime is already updated)
                fromSqlVersion = GetSqlToursVersion();
                cache.Remove("Version");
                cache.Add("Version", fromSqlVersion, DateTime.MaxValue, new TimeSpan(24, 0, 0));

                //update the WWTTours cache with the SQLTours ArrayList
                cache.Remove("WWTXMLTours");
                cache.Add("WWTXMLTours", LoadTourHierarchy(), DateTime.MaxValue, new TimeSpan(24, 0, 0));
            }
            return 0;
        }
    }
}
