namespace WWT.Providers
{
    public class postmarsdemProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            context.Response.Write("OK");
        }
    }
}
