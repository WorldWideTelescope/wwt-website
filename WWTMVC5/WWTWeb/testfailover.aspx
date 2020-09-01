<%@ Page Language="C#" ContentType="text/html" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<testfailoverProvider>().Run(this);
%>
