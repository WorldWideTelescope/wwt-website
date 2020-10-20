<%@ Page Language="C#" Async="true" ContentType="application/octet-stream" %>
<%@ Import Namespace="WWT.Providers" %>

<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		RequestProviderRunner.Run<moontoastdemProvider>(this);
	}
</script>
