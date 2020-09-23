<%@ Page Language="C#" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<GetToursProvider>().Run(this);
%>
