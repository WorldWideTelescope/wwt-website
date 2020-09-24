<%@ Page Language="C#" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<GetTourListProvider>().Run(this);
%>
