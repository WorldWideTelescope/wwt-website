using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for SearchResults
/// </summary>
public class SearchResults
{
    public int TotalResults = 0;
    public List<string> Suggestions = new List<string>();
    public List<ResultItem> WebResults = new List<ResultItem>();
}

public class ResultItem
{
    public ResultItem(string url, string title, string description)
    {
        this.Url = url;
        this.Title = title;
        this.Description = description;
    }

    private string _url;
    public string Url
    {
        get { return _url; }
        set { _url = value; }
    }

    private string _title;
    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    private string _description;
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }	
}

