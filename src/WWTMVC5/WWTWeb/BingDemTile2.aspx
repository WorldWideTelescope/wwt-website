<%@ Page Language="C#" ContentType="application/octet-stream" %>

<%@ Import Namespace="WWT.Providers" %>
<%
    RequestProvider.Get<BingDemTile2Provider>().Run(this);
%>