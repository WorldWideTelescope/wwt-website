using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using WWT.Web;

var builder = WebApplication.CreateBuilder(args);

// This configuration is used to to identify the tenant of the KeyVault. If the user
// is not in the same tenant, then the token will be incorrect. The error message
// you'll see will be similar to: AKV10032: Invalid issuer
var tenant = builder.Configuration["DefaultCredentialTenantId"];

var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    VisualStudioCodeTenantId = tenant,
    VisualStudioTenantId = tenant
});

if (builder.Configuration["KeyVaultName"] is { } keyVaultName && !string.IsNullOrEmpty(keyVaultName))
{
    var uri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(uri, credential);
}

builder.AddServiceDefaults();
builder.AddWwt();

builder.Services.AddSingleton(credential);

builder.Services.AddCors(options => options
    .AddDefaultPolicy(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

builder.Services.AddMvcCore();

builder.Services.AddLogging(builder =>
{
    builder.AddFilter("Swick.Cache", LogLevel.Trace);
    builder.AddDebug();
});

builder.Services.AddSingleton(typeof(HelloWorldProvider));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();

app.UseRouting();

app.MapWwt();

app.Run();