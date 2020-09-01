<%@ Page Language="C#" ContentType="text/plain" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<HiriseDem2Provider>().Run(this);
%>
