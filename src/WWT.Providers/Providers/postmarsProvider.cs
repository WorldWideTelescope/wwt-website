namespace WWT.Providers
{
    public class postmarsProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            context.Response.Write("OK");
        }
    }
}
