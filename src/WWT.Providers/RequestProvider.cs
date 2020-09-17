using System;
using System.Web.UI;

namespace WWT.Providers
{
    public abstract class RequestProvider
    {
        private static IServiceProvider _provider;

        public void Run(Page page)
            => Run(new WwtContext(page));

        public abstract void Run(WwtContext context);

        public static RequestProvider Get<TProvider>()
            where TProvider : RequestProvider
            => (RequestProvider)_provider.GetService(typeof(TProvider));

        public static void SetServiceProvider(IServiceProvider provider)
            => _provider = provider;
    }
}
