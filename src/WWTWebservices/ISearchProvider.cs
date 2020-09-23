using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for ISearchProvider
/// </summary>
public interface ISearchProvider
{
    SearchResults ExecuteQuery(string queryText, string culture, 
        int resultsPerPage, int resultsPageIndex, bool hightlight);
}
