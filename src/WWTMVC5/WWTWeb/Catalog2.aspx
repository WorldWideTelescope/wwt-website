<%@ Page Language="C#" ContentType="text/plain" %>

<%@ Import Namespace="WWT.Providers" %>
<%
    RequestProvider.Get<Catalog2Provider>().Run(this);
%>