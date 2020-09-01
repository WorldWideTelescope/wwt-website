using System.Web.UI;

namespace WWT.Providers
{
    public abstract class RequestProvider
    {
        public void Run(Page page)
            => Run(new WwtContext(page));

        public abstract void Run(WwtContext context);

        public static RequestProvider Get<TProvider>()
            where TProvider : RequestProvider, new()
            => new TProvider();
    }
}
