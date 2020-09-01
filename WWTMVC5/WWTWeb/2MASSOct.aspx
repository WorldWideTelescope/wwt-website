<%@ Page Language="C#" ContentType="image/png" %>

<%@ Import Namespace="WWT.Providers" %>
<%
    RequestProvider.Get<TwoMASSOctProvider>().Run(this);
%>