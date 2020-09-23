<%@ Page Language="C#" ContentType="application/x-wtt" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<GetTourProvider>().Run(this);
%>
