using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Microsoft.Research.WWTLib
{
    public class ObjectDetails
    {
        SqlConnections oSqlConnections;
        internal static string dbName;
        public SqlDataReader GetObjectDetails(int id)
        {
            try
            {
                dbName = "AstroObjects";
                oSqlConnections = new SqlConnections();

                return SqlHelper.ExecuteReader(oSqlConnections.GetSqlConnection(dbName), "spGetAstroObjectDetails", id);
            }
            catch
            {
                throw;
            }
        }

    }
}
