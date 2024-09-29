namespace Personal.BlobStorage.Domain
{
    public class RiseCosmosDbOptions
    {
        public Uri AccountEndpoint { get; set; } = null!;
        public string DatabaseName { get; set; } = "rise-assessment";
        public string BlobFileInfoRepositoryName { get; set; } = "blob-file-info";
        public string LeaseContainerName { get; set; } = "lease";
        public string ProcessorName { get; set; } = "rise-assessment-srikanth-home";
        public string InstanceName { get; set; } = "rise-assessment";
    }
}
