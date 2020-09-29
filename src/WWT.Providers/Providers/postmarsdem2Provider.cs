namespace WWT.Providers
{
    public class postmarsdem2Provider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            context.Response.Write("OK");
        }
    }
}
