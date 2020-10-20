<%@ Page Language="C#" Async="true" ContentType="image/jpg" %>
<%@ Import Namespace="WWT.Providers" %>

<script runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		RequestProviderRunner.Run<GetAuthorThumbnailProvider>(this);
	}
</script>
