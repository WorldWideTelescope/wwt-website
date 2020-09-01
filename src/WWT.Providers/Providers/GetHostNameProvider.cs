namespace WWT.Providers
{
    public class GetHostNameProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            context.Response.Write(context.Server.MachineName);
        }
    }
}
