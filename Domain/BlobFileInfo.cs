using System.Text.Json.Serialization;
namespace Personal.BlobStorage.Domain
{
    public class BlobFileInfo
    {
        public BlobFileInfo(string blobFileName, Dictionary<string, string>? metadata = null)
        {
            Id = blobFileName ?? throw new ArgumentNullException(nameof(blobFileName));
            BlobFileName = blobFileName ?? throw new ArgumentNullException(nameof(blobFileName));
            Metadata = metadata;
        }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        public string BlobFileName { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
