<%@ Page Language="C#" ContentType="text/plain" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<isstle2Provider>().Run(this);
%>
