<%@ Page Language="C#" Async="true" ContentType="text/plain" %>
<%@ Import Namespace="WWT.Providers" %>

<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		RequestProviderRunner.Run<GetHostNameProvider>(this);
	}
</script>
