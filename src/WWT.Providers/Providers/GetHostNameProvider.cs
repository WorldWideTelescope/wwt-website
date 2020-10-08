namespace WWT.Providers
{
    public class GetHostNameProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            context.Response.Write(context.MachineName);
        }
    }
}
