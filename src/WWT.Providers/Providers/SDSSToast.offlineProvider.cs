namespace WWT.Providers
{
    public class SDSSToastOfflineProvider : RequestProvider
    {
        public override void Run(WwtContext context)
        {
            context.Response.Write("Hello");
        }
    }
}
