<%@ WebHandler Language="C#" Class="IssuesAndBugs" %>

using System.Collections.Specialized;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web;

public class IssuesAndBugs : IHttpHandler {
	
	public void ProcessRequest (HttpContext context) {
		NameValueCollection json = context.Request.Params;
		StringBuilder msg = new StringBuilder();
		msg.Append("<style>strong,h1,h3,td{font-family:Segoe UI;}</style><h1>Issue Report</h1><hr/><h3>Issue Information</h3>");
		msg.Append("<table>");
		AddRow(msg, "Full Name", json.Get("fullName"));
		AddRow(msg, "Issue Type", json.Get("issueType"));
		
		AddRow(msg, "Email", json.Get("email"));
		AddRow(msg, "Comments", json.Get("comments"));
		
		msg.Append("</table>");
		SendMail(
				ConfigurationManager.AppSettings["SmtpServer"],
				ConfigurationManager.AppSettings["SupportEmailAddress"],
				ConfigurationManager.AppSettings["SupportEmailAddress"],
				"WWT Issue/Bug Report",
				msg.ToString()
			);
	}

	private StringBuilder AddRow(StringBuilder sb, string lbl, string v)
	{
		sb.AppendFormat("<tr><td><strong>{0}:</strong></td><td>{1}</td></tr>", lbl, v);
		return sb;
	}

	/// <summary>
	/// Generic SMPT Email Sender.
	/// </summary>
	/// <param name="smtpServer"></param>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <param name="subject"></param>
	/// <param name="body"></param>
	protected void SendMail(string smtpServer, string from, string to, string subject, string body)
	{
		MailAddress oFrom = new MailAddress(from);
		MailAddress oTo = new MailAddress(to);

		MailMessage message = new MailMessage();
		message.To.Add(oTo);
		message.From = oFrom;
		message.Subject = subject;
		message.IsBodyHtml = true;
		message.Body = body;

		SmtpClient mailClient = new SmtpClient(smtpServer);
		mailClient.Send(message);
	}

	public bool IsReusable {
		get {
			return false;
		}
	}

}