<%@ Page Language="C#" ContentType="image/png" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<GlimpseProvider>().Run(this);
%>
