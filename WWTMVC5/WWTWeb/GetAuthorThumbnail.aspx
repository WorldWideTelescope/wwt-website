<%@ Page Language="C#" ContentType="image/jpg" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<GetAuthorThumbnailProvider>().Run(this);
%>
