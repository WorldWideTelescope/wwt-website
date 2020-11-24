#nullable disable

using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace WWT.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static TokenCredential _credential;

        private static TokenCredential BuildTokenCredential(IConfiguration config)
        {
            if (_credential is null)
            {
                // This configuration is used to to identify the tenant of the KeyVault. If the user
                // is not in the same tenant, then the token will be incorrect. The error message
                // you'll see will be similar to: AKV10032: Invalid issuer
                var tenant = config["DefaultCredentialTenantId"];

                _credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    VisualStudioCodeTenantId = tenant,
                    VisualStudioTenantId = tenant
                });
            }

            return _credential;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(BuildTokenCredential(context.Configuration));
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();
                    var keyVaultName = builtConfig["KeyVaultName"];

                    if (!string.IsNullOrEmpty(keyVaultName))
                    {
                        var uri = new Uri($"https://{keyVaultName}.vault.azure.net/");
                        config.AddAzureKeyVault(uri, BuildTokenCredential(builtConfig));
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
