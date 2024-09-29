using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Personal.BlobStorage.Infrastructure;
using Personal.BlobStorage.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Azure.Identity;
using Personal.BlobStorage.Infrastructure.HostedService;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Text.Json;
using Domain;

var builder = WebApplication.CreateBuilder(args);


Person person = new Person("srikanth", DateTime.Parse("1999-09-06"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAzureAppConfiguration();

builder.Configuration.AddAzureAppConfiguration(options =>
{
    var appConfigEndpoint = builder.Configuration["AppConfig:AccountEndpoint"];

    options.Connect(new Uri(appConfigEndpoint!), new DefaultAzureCredential())
            .Select("student-service:*", builder.Configuration["AppConfig:Label"])
            .ConfigureRefresh(refreshOptions =>
            {
                refreshOptions.Register("student-service.NamebasedUUID:EnvironmentId", refreshAll: true)
                       .SetCacheExpiration(TimeSpan.FromSeconds(10));
            }
           );
});

builder.Services.Configure<NameBasedUUIDOptions>(builder.Configuration.GetSection("student-service.NamebasedUUID") ?? builder.Configuration.GetSection("NamebasedUUID"));


//builder.Configuration.AddAzureAppConfiguration(options =>
//{
//    var appConfigEndpoint = builder.Configuration["AppConfig:AccountEndpoint"]!;
//    options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential())
//    .UseFeatureFlags();
//    builder.Services.AddSingleton(options.GetRefresher());
//});


//builder.Configuration.AddAzureAppConfiguration(options =>
//{
//    var url = builder.Configuration.GetSection("AppConfig:AccountEndpoint").Value;
//    options.Connect(new Uri(url!), new DefaultAzureCredential())
//           // Load all keys that start with `TestApp:` and have no label
//           .Select("student-service:*", "dev")
//           // Configure to reload configuration if the registered sentinel key is modified
//           .ConfigureRefresh(refreshOptions =>
//                refreshOptions.Register("student-service.NamebasedUUID.EnvironmentId", refreshAll: true));
//});

//var strName = builder.Configuration[" student-service.NamebasedUUID:EnvironmentId"];

//builder.Services.Configure<NameBasedUUIDOptions>(builder.Configuration.GetSection(" student-service.NamebasedUUID"));

//IConfiguration configuration = new ConfigurationBuilder()
//    .AddAzureAppConfiguration(options =>
//    {
//        options.Connect(Environment.GetEnvironmentVariable("ConnectionString"))
//            .UseFeatureFlags();
//    }).Build();
//IFeatureDefinitionProvider featureDefinitionProvider = new ConfigurationFeatureDefinitionProvider(configuration);

//IFeatureManager featureManager = new FeatureManager(
//    featureDefinitionProvider,
//    new FeatureManagementOptions());

builder.Services.AddFeatureManagement();

builder.Services.AddOptions<RiseCosmosDbOptions>() 
    .Bind(builder.Configuration.GetSection("Azure:RiseCosmosDB"));

builder.Services.AddSingleton(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<RiseCosmosDbOptions>>().Value;
    return new CosmosClientBuilder(options.AccountEndpoint.AbsoluteUri, new DefaultAzureCredential())
        .WithApplicationName("rise-assessment")
        .WithSerializerOptions(new CosmosSerializationOptions() 
            { 
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
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

//builder.Services.AddSingleton(provider =>
//{
//    var refreshers = provider.GetRequiredService<IConfigurationRefresherProvider>().Refreshers;
//    return refreshers.First();
//});
//builder.Services.AddHostedService<AzureAppConfigRefreshService>();
builder.Services.AddSingleton<ILibraryUtilityService, LibraryUtilityService>();
builder.Services.AddSingleton<IProductFileRepository, ProductFileRepository>();

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
