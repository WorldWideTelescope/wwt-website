namespace WWT.Providers
{
    public class postmarsdemProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            context.Response.Write("OK");
        }
    }
}
