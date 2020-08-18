using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Research.WWT;

/// <summary>
/// Summary description for WebContentServices
/// </summary>
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