<%@ Page Language="C#" ContentType="application/x-wtml" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<ShowImage2Provider>().Run(this);
%>
