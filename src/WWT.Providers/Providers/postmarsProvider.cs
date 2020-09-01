namespace WWT.Providers
{
    public class postmarsProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            context.Response.Write("OK");
        }
    }
}
