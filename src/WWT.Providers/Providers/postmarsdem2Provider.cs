namespace WWT.Providers
{
    public class postmarsdem2Provider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            context.Response.Write("OK");
        }
    }
}
