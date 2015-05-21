using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Class1
/// </summary>
///
namespace WWTWebServices
{
    public static class Database
    {
        internal static SqlConnection GetConnectionAstroObjects()
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

        public static SqlConnection GetConnectionWWTPageContent()
        {
            string connStr = null;
            connStr = ConfigurationManager.AppSettings["WWTPageContentDBConnectionString"];
            SqlConnection myConnection = null;
            myConnection = new SqlConnection(connStr);
            return myConnection;
        }

	}

}