using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using WebUtil;
using Database = WebServices.Database;

/// <summary>
/// Summary description for WebContentServices
/// </summary>
/// 

namespace WebUtil
{


    public static class WebContent
    {

        public static List<ContentHomePage> HomePage()
        {
            List<ContentHomePage> content = new List<ContentHomePage>();
            ContentHomePage homePage;

            SqlConnection con = Database.GetConnectionWWTTours();

            DataSet ds = new DataSet();

            try
            {

                StoredProc sproc = new StoredProc("spHomePageSelect", con);
                DataTable table = new DataTable();
                sproc.RunQuery(table);
                foreach (DataRow row in table.Rows)
                {
                    homePage = new ContentHomePage(row);

                    content.Add(homePage);
                }
            }
            catch (SqlException sqlErr)
            {
                throw (sqlErr);
            }

            return content;
        }
    }
}