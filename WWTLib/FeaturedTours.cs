using System;
using System.Collections.Generic;
////using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Microsoft.Research.WWTLib
{
    public class FeaturedTours
    {
        SqlConnections oSqlConnections;
        internal static string dbName;
        public SqlDataReader GetFeaturedWWTTours()
        {
            try
            {
                dbName = "WWTTours";
                oSqlConnections = new SqlConnections();

                return SqlHelper.ExecuteReader(oSqlConnections.GetSqlConnection(dbName), "spGetFeaturedWWTTours");
            }
            catch
            {
                throw;
            }
        }
    }
}
