using System;
using System.Web.UI;

namespace WWT.Providers
{
    public static class RequestProviderRunner
    {
        private static IServiceProvider _provider;

        public static void Run<TProvider>(Page page)
            where TProvider : RequestProvider
        {
            var provider =  (RequestProvider)_provider.GetService(typeof(TProvider));
            page.RegisterAsyncTask(new PageAsyncTask(token => provider.RunAsync(new PageWwtContext(page), token)));
            page.ExecuteRegisteredAsyncTasks();
        }

        public static void SetServiceProvider(IServiceProvider provider)
            => _provider = provider;
    }
}
