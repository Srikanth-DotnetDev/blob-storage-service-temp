using System.Text.Json.Serialization;
namespace Personal.BlobStorage.Domain
{
    public class BlobFileInfo
    {
        public BlobFileInfo(string blobFileName, Dictionary<string, string>? metaData = null)
        {
            Id = blobFileName ?? throw new ArgumentNullException(nameof(blobFileName));
            BlobFileName = blobFileName ?? throw new ArgumentNullException(nameof(blobFileName));
            MetaData = metaData;
        }
        public string Id { get; set; }
        public string BlobFileName { get; set; }
        public Dictionary<string, string>? MetaData { get; set; }
    }
}
