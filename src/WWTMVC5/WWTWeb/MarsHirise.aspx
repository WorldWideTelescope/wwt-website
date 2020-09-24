<%@ Page Language="C#" ContentType="image/png" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<MarsHiriseProvider>().Run(this);
%>
