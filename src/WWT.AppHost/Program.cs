using Projects;
using WWT.AppHost;
using System.Runtime.InteropServices;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache")
    .WithRedisCommander();

var storage = builder.AddAzureStorage("wwtstorage")
    .RunAsEmulator();

var wwtFiles = storage.AddBlobs("WwtFiles")
    .WithDevData();
var mars = storage.AddBlobs("Mars");

var webclient = builder.AddDockerfile("webclient", "webclient/")
    .WithHttpEndpoint(targetPort: 8080)
    .ExcludeFromManifest();

// This container only supports x86_64
if (RuntimeInformation.OSArchitecture is Architecture.X64)
{
    builder.AddContainer("storage-explorer", "sebagomez/azurestorageexplorer")
        .WithHttpEndpoint(targetPort: 8080)
        .WithEnvironment(e =>
        {
            e.EnvironmentVariables["AZURE_STORAGE_CONNECTIONSTRING"] = new ConnectionStringReference(wwtFiles.Resource, optional: false);
        })
        .ExcludeFromManifest();
}

builder.AddProject<WWT_Web>("web")
    .WithReference(wwtFiles)
    .WithReference(mars)
    .WithReference(redis);

builder.Build().Run();
