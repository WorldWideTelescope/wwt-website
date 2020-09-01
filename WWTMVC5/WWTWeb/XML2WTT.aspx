<%@ Page Language="C#" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<XML2WTTProvider>().Run(this);
%>
