<%@ Page Language="C#" ContentType="image/jpeg" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<mandel1Provider>().Run(this);
%>
