<%@ Page Language="C#" ContentType="image/png" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<tiles2Provider>().Run(this);
%>
