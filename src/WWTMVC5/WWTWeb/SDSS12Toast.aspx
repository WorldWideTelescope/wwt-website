<%@ Page Language="C#" ContentType="image/png" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<SDSS12ToastProvider>().Run(this);
%>
