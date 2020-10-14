<%@ Page Language="C#" ContentType="image/jpeg" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
    RequestProvider.Get<TilethumbProvider>().Run(this);
%>
