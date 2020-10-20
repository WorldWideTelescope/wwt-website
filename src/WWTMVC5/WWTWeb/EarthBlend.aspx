<%@ Page Language="C#" Async="true" ContentType="image/png" %>
<%@ Import Namespace="WWT.Providers" %>

<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		RequestProviderRunner.Run<EarthBlendProvider>(this);
	}
</script>
