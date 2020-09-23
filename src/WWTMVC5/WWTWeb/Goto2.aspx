<%@ Page Language="C#" ContentType="application/x-wtml" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<Goto2Provider>().Run(this);
%>
