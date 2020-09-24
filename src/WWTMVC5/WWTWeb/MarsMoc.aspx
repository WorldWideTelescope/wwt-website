<%@ Page Language="C#" ContentType="image/png" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<MarsMocProvider>().Run(this);
%>
