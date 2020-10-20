<%@ Page Language="C#" Async="true" ContentType="image/jpeg" %>
<%@ Import Namespace="WWT.Providers" %>

<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		RequestProviderRunner.Run<mandel1Provider>(this);
	}
</script>
