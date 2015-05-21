using System;
using System.Collections.Generic;
////using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Microsoft.Research.WWTLib
{
    public class RecentWWTTours
    {
        SqlConnections oSqlConnections;
        internal static string dbName;
        public SqlDataReader GetRecentWWTTours()
        {
            try
            {
                dbName = "WWTTours";
                oSqlConnections = new SqlConnections();

                return SqlHelper.ExecuteReader(oSqlConnections.GetSqlConnection(dbName), "spGetMostRecentWWTTours", 4, "approved");
            }
            catch
            {
                throw;
            }
        }
    }
}
