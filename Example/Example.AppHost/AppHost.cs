using Microsoft.Extensions.Azure;
using static System.Reflection.Metadata.BlobBuilder;

var builder = DistributedApplication.CreateBuilder(args);

var blobService = builder.AddAzureStorage("ApiServiceStorage").RunAsEmulator(x =>
{
    x.WithDataVolume();
});
var inputService = builder.AddAzureStorage("InputServiceStorage").RunAsEmulator(x =>
{
    x.WithDataVolume();
});

var apiStorage = blobService.AddBlobs("BlobStorage");
var inputStorage = inputService.AddBlobs("InputStorage");

var apiService = builder
    .AddProject<Projects.Example_ApiService>("api-service")
    //.WaitFor(apiStorage)
    .WithEnvironment("ConnectionStrings__BlobStorage", apiStorage)
    .WithReference(apiStorage)
    .WithEnvironment("ConnectionStrings__InputStorage", inputStorage)
    .WithReference(inputStorage);

builder.Build().Run();
