<%@ Page Language="C#" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<PostRatingFeedbackProvider>().Run(this);
%>
