<%@ Page Language="C#" Async="true" %> 
<%@ Import Namespace="WWT.Providers" %>

<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		RequestProviderRunner.Run<XML2WTTProvider>(this);
	}
</script>
