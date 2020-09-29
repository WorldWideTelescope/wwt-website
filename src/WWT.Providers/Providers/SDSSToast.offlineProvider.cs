namespace WWT.Providers
{
    public class SDSSToastOfflineProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            context.Response.Write("Hello");
        }
    }
}
