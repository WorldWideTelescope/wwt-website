<%@ Page Language="C#" Async="true" ContentType="application/x-wtml" %>
<%@ Import Namespace="WWT.Providers" %>

<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		RequestProviderRunner.Run<TileImageProvider>(this);
	}
</script>
