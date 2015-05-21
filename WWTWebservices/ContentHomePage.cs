using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace WWTWebservices
{


/// <summary>
/// Summary description for ContentHomePage
/// </summary>
public class ContentHomePage
{
	public ContentHomePage(DataRow row)
	{
        if (row["Title"] != DBNull.Value)
            _title = row["Title"].ToString();

        if (row["Content"] != DBNull.Value)
            _content = row["Content"].ToString();
	}

    private string _title;
    public string Title { get { return _title; } set { _title = value; } }

    private string _content;
    public string Content { get { return _content; } set { _content = value; } }
}
}