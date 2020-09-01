<%@ Page Language="C#" ContentType="application/x-wtml" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<GotoProvider>().Run(this);
%>
