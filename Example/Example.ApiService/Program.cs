using Example.ApiService.Extensions;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddAzureBlobClient("BlobStorage",
    //configureSettings: configureSettings => { configureSettings.DisableHealthChecks = true; }, // When commented out, causes "InvalidOperationException: Unable to find client registration with type 'BlobServiceClient' and name 'Default'." 
    configureClientBuilder: configureClientBuilder =>
    {
        configureClientBuilder.WithName("API");
    }
);

builder.AddAzureBlobClient("InputStorage",
    //configureSettings: configureSettings => { configureSettings.DisableHealthChecks = true; }, // When commented out, causes "InvalidOperationException: Unable to find client registration with type 'BlobServiceClient' and name 'Default'." 
    configureClientBuilder: configureClientBuilder =>
    {
        configureClientBuilder.WithName("Input");
    }
);

builder.Services.AddHealthChecks()
    .AddAzureBlobStorage((services) => services.GetRequiredService<IAzureClientFactory<Azure.Storage.Blobs.BlobServiceClient>>().CreateClient("API"), name: "apistorage", tags: new[] { "storage" })
    .AddAzureBlobStorage((services) => services.GetRequiredService<IAzureClientFactory<Azure.Storage.Blobs.BlobServiceClient>>().CreateClient("Input"), name: "inputstorage", tags: new[] { "storage" });

var app = builder.Build();

app.MapDefaultEndpoints();

app.SetupHealthCheck();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
