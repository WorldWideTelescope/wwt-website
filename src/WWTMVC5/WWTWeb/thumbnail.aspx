<%@ Page Language="C#" ContentType="image/jpeg" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<ThumbnailProvider>().Run(this);
%>
