namespace Personal.BlobStorage.Domain
{
    public class RiseCosmosDbOptions
    {
        public Uri AccountEndpoint { get; set; } = null!;
        public string DatabaseName { get; set; } = "rise-assessment";
        public string BlobFileInfoRepositoryRepositoryName { get; set; } = "blob-file-info";
    }
}
