using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("wwtstorage")
    .RunAsEmulator(builder => builder.WithImageTag("3.32.0"));

var wwtFiles = storage.AddBlobs("WwtFiles");
var mars = storage.AddBlobs("Mars");

builder.AddProject<WWT_Web>("web")
    .WithReference(wwtFiles)
    .WithReference(mars);

builder.Build().Run();
