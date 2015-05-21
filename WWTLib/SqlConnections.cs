using System;
using System.Collections.Generic;
////using System.Linq;
using System.Text;
using System.Configuration;

namespace Microsoft.Research.WWTLib
{
    class SqlConnections
    {
        public string GetSqlConnection(string dbName)
        {
            //Add logic to fetch the connection strings from the registry
            switch (dbName)
            {
                case ("WWTTours"):
                    return ConfigurationManager.AppSettings["ToursConn"];
                case ("AstroObjects"):
                    return ConfigurationManager.AppSettings["ObjectsConn"];
                default:
                    return "";
            }
        }
    }
}
