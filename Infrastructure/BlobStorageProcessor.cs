using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos;
using Personal.BlobStorage.Domain;


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
            //_blobFileEventHandler = blobFileEventHandler;

            var blobContainer = cosmosClient.GetContainer(options.Value.DatabaseName, options.Value.BlobFileInfoRepositoryRepositoryName);

            var leaseContainer = cosmosClient.GetContainer(options.Value.DatabaseName, options.Value.LeaseContainerName);


            _cfp = blobContainer.GetChangeFeedProcessorBuilder<BlobFileInfo>(options.Value.ProcessorName, ProcessChanges)
                                       .WithLeaseContainer(leaseContainer)
                                       .WithInstanceName(options.Value.InstanceName)
                                       .WithStartTime(DateTime.Now.AddDays(-365))
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
        private async Task ProcessChanges(IReadOnlyCollection<BlobFileInfo>? blobFileInfos, CancellationToken cancellationToken)
        {
            if ( blobFileInfos == null || blobFileInfos.Count == 0 ) { return; }
            foreach (var blobFileInfo in blobFileInfos)
            {
                await _blobFileEventHandler.Handle(blobFileInfo);
            }
                
        }
    }
}
