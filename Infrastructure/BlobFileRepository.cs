using Personal.BlobStorage.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;

namespace Personal.BlobStorage.Infrastructure
{
    public class BlobFileInfoRepository : IBlobFileInfoRepository
    {
        private readonly Container _container;
        private IOptions<RiseCosmosDbOptions> _options;

        public BlobFileInfoRepository(CosmosClient cosmosClient, IOptions<RiseCosmosDbOptions> options)
        {
            _options = options;
            if (cosmosClient is null)
            {
                throw new ArgumentNullException(nameof(cosmosClient));
            }
            _container = cosmosClient.GetContainer(_options.Value.DatabaseName, _options.Value.BlobFileInfoRepositoryRepositoryName);
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
                throw ex;
            }
        }
    }
}
