using Personal.BlobStorage.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace Personal.BlobStorage.Infrastructure
{
    public class BlobFileInfoRepository : IBlobFileInfoRepository
    {
        private readonly Container _container;
        private IOptions<RiseCosmosDbOptions> _options;
        private ILogger<BlobFileInfoRepository> _logger;

        public BlobFileInfoRepository(CosmosClient cosmosClient, IOptions<RiseCosmosDbOptions> options, ILogger<BlobFileInfoRepository> logger)
        {
            _options = options;
            if (cosmosClient is null)
            {
                throw new ArgumentNullException(nameof(cosmosClient));
            }
            _container = cosmosClient.GetContainer(_options.Value.DatabaseName, _options.Value.BlobFileInfoRepositoryName);
            _logger = logger;
        }

        public async Task<BlobFileInfo?> GetBlobFileInfoAsync(string blobFileName)
        {
            try
            {
                var response = await this._container.
                    ReadItemAsync<BlobFileInfo>(blobFileName, new PartitionKey(blobFileName));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "some exception while retrieving the blob file info");
                return null;
            }
            
        }
        public async Task UpsertBlobFileInfo(BlobFileInfo blobFileInfo)
        {
            try
            {
                await _container.UpsertItemAsync(item: blobFileInfo, partitionKey: new PartitionKey(blobFileInfo.BlobFileName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "some exception while upserting the blob file info");
                return;
            }
        }
    }
}
