using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Personal.BlobStorage.Infrastructure;
using Personal.BlobStorage.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Azure.Identity;
using Personal.BlobStorage.Infrastructure.HostedService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<RiseCosmosDbOptions>()
    .Bind(builder.Configuration.GetSection("Azure:RiseCosmosDB"));

builder.Services.AddSingleton(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<RiseCosmosDbOptions>>().Value;
    return new CosmosClientBuilder(options.AccountEndpoint.AbsoluteUri, new DefaultAzureCredential()).WithConnectionModeDirect(portReuseMode : PortReuseMode.ReuseUnicastPort)
        .WithApplicationName("rise-assessment")
        .WithSerializerOptions(new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
            IgnoreNullValues = true
        })
    .Build();
});
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage") ?? "DefaultEndpointsProtocol=https;AccountName=blobsftpdemo3;AccountKey=6t3gR4YDX/3rad17BOT3ix0awaeD9xZBRoSix3CkfZZRelO2xHhNK+1Itt2twqb8xD2j5apNZyM3+AStYEaJdw==;EndpointSuffix=core.windows.net"));
//builder.Services.AddHostedService<Worker>();

//builder.Services.AddSingleton<IBlobClientUtilityService>(serviceProvider =>
//{
//    var options = builder.Configuration.GetConnectionString("AzureBlobStorage") ?? "DefaultEndpointsProtocol=https;AccountName=blobsftpdemo3;AccountKey=6t3gR4YDX/3rad17BOT3ix0awaeD9xZBRoSix3CkfZZRelO2xHhNK+1Itt2twqb8xD2j5apNZyM3+AStYEaJdw==;EndpointSuffix=core.windows.net";
//    var blobServiceClient = new BlobServiceClient(options);
//    return ActivatorUtilities.CreateInstance<BlobClientUtilityService>(serviceProvider, blobServiceClient);
//});

//builder.Services.AddOptions<RiseCosmosDbOptions>()
//    .Bind(builder.Configuration.GetSection("Azure:RiseCosmosDb"));
builder.Services.AddSingleton<IBlobFileEventHandler, BlobFileEventHandler>();


builder.Services.AddSingleton<IBlobFileInfoRepository, BlobFileInfoRepository>();
builder.Services.AddSingleton<IBlobClientUtilityService, BlobClientUtilityService>();
builder.Services.AddHostedService<BlobStorageProcessor>();
builder.Services.AddSingleton<ILibraryUtilityService, LibraryUtilityService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//using (var scope = app.Services.CreateScope())
//{
//    var worker = scope.ServiceProvider.GetRequiredService<IWorker>();
//    await worker.GetNames();
//}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
