<%@ Page Language="C#" Async="true" ContentType="application/x-wtt" %>
<%@ Import Namespace="WWT.Providers" %>

<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		RequestProviderRunner.Run<GetTourProvider>(this);
	}
</script>
