using System;
using System.Collections.Generic;
////using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Microsoft.Research.WWTLib
{
    public class SForO
    {
        SqlConnections oSqlConnections;
        internal static string dbName;
        public SqlDataReader GetAstroObjects
        (
            string pSearchString,
            string pRa,
            string pDec,
            string pPlusMinusArcSecs,
            string pVMagThreshold,
            int pConstellationID, 
            int pObjectTypeUIListID
        )
        {    
            try
            {
                dbName = "AstroObjects";
                oSqlConnections = new SqlConnections();

                return SqlHelper.ExecuteReader(oSqlConnections.GetSqlConnection(dbName), "spGetAstroObjectsForWeb", pSearchString, pRa, pDec, pPlusMinusArcSecs, pVMagThreshold, pConstellationID, pObjectTypeUIListID);
            }
            catch 
            {
                throw;
            }
        }


        public SqlDataReader GetObjectTypeList()
        {
            try
            {
                dbName = "AstroObjects";
                oSqlConnections = new SqlConnections();

                return SqlHelper.ExecuteReader(oSqlConnections.GetSqlConnection(dbName), "spGetObjectTypesList");
            }
            catch
            {
                throw;
            }
        }

        public SqlDataReader GetConstellationList()
        {
            try
            {
                dbName = "AstroObjects";
                oSqlConnections = new SqlConnections();

                return SqlHelper.ExecuteReader(oSqlConnections.GetSqlConnection(dbName), "spGetConstellationList");
            }
            catch
            {
                throw;
            }
        }


    }
}
