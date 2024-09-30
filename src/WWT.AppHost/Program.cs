using Projects;
using WWT.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache")
    .WithRedisCommander();

var storage = builder.AddAzureStorage("wwtstorage")
    .RunAsEmulator(builder => builder.WithImageTag("3.32.0"));

var wwtFiles = storage.AddBlobs("WwtFiles")
    .WithDevData();
var mars = storage.AddBlobs("Mars");

var webclient = builder.AddDockerfile("webclient", "webclient/")
    .WithHttpEndpoint(targetPort: 8080)
    .ExcludeFromManifest();

builder.AddContainer("storage-explorer", "sebagomez/azurestorageexplorer")
    .WithHttpEndpoint(targetPort: 8080)
    .WithEnvironment(e =>
    {
        e.EnvironmentVariables["AZURE_STORAGE_CONNECTIONSTRING"] = new ConnectionStringReference(wwtFiles.Resource, optional: false);
    })
    .ExcludeFromManifest();

builder.AddProject<WWT_Web>("web")
    .WithReference(wwtFiles)
    .WithReference(mars)
    .WithReference(redis);

builder.Build().Run();
