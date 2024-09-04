using Personal.BlobStorage.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace Personal.BlobStorage.Infrastructure
{
    public interface IProductFileRepository
    {
         Task UpsertItemAsync();
    }
    public class ProductFileRepository : IProductFileRepository
    {
        private readonly Container _container;
        private ILogger<ProductFileRepository> _logger;
        private readonly string[] ProductTypes = { "Laptop", "SmartPhones", "Ipad" }; 

        public ProductFileRepository(CosmosClient cosmosClient, ILogger<ProductFileRepository> logger)
        {
            if (cosmosClient is null)
            {
                throw new ArgumentNullException(nameof(cosmosClient));
            }
            _container = cosmosClient.GetContainer("rise-assessment", "products");
            _logger = logger;
        }

       public async Task UpsertItemAsync()
        {
            for(int i=0; i<5; i++) {
                 var productInfo = new ProductInfo(Guid.NewGuid().ToString(), ProductTypes[Random.Shared.Next(ProductTypes.Length)]);
                await _container.UpsertItemAsync(item : productInfo, partitionKey : new PartitionKey(productInfo.productType));
            }
        }
    }
}
