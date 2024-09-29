using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos;
using Personal.BlobStorage.Domain;
using System.Text.Json;
using Azure.Messaging;


namespace Personal.BlobStorage.Infrastructure.HostedService
{
    public class BlobStorageProcessor : IHostedService
    {
        private readonly ChangeFeedProcessor _cfp;
        private readonly ILogger<BlobStorageProcessor> _logger;
        private readonly IBlobFileEventHandler _blobFileEventHandler;

        public BlobStorageProcessor(ILogger<BlobStorageProcessor> logger, IOptions<RiseCosmosDbOptions> options, IBlobFileEventHandler blobFileEventHandler, CosmosClient cosmosClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var blobContainer = cosmosClient.GetContainer(options.Value.DatabaseName, options.Value.BlobFileInfoRepositoryName);

            var leaseContainer = cosmosClient.GetContainer(options.Value.DatabaseName, options.Value.LeaseContainerName);


            _cfp = blobContainer.GetChangeFeedProcessorBuilder<CloudEvent>(options.Value.ProcessorName, ProcessChanges)
                                       .WithLeaseContainer(leaseContainer)
                                       .WithInstanceName(options.Value.InstanceName)
                                       .WithStartTime(DateTime.MinValue.ToUniversalTime())
                                       .Build();

            _blobFileEventHandler = blobFileEventHandler ?? throw new ArgumentNullException( nameof(blobFileEventHandler));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _cfp.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _cfp.StopAsync();
        }
        private async Task ProcessChanges(IReadOnlyCollection<CloudEvent>? cloudEvents, CancellationToken cancellationToken)
        {
            if(cloudEvents is null || !cloudEvents.Any())
            {
                _logger.LogInformation("There are no objects to processs");
                return;
            }
            _logger.LogInformation("Starting to process the blob-file-info");
            foreach (var cloudEvent in cloudEvents)
            {
                try
                {
                    if(cloudEvent is null or { Data : null})
                    {
                        _logger.LogInformation("The event data is null");
                        continue;
                    }

                    var blobFileInfo = JsonSerializer.Deserialize<BlobFileInfo>(cloudEvent.Data.ToString()!);

                    if(blobFileInfo is null)
                    {
                        _logger.LogInformation("An issue occured while deserializing cloud event data into blob-file-info");
                        continue;
                    }
                    await _blobFileEventHandler.Handle(blobFileInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the blob-file-info");
                    continue;
                }
            }
            _logger.LogInformation("Successfully processed blob-file-info");
        }
    }
}

