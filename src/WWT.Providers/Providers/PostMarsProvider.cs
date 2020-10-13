namespace WWT.Providers
{
    public class PostMarsProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            context.Response.Write("OK");
        }
    }
}
