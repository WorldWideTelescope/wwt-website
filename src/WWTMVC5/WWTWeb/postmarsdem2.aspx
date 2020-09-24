<%@ Page Language="C#" ContentType="text/plain" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<postmarsdem2Provider>().Run(this);
%>
