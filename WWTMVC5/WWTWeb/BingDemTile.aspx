<%@ Page Language="C#" ContentType="application/octet-stream" %>

<%@ Import Namespace="WWT.Providers" %>
<%
    RequestProvider.Get<BingDemTileProvider>().Run(this);
%>