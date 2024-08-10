using Personal.BlobStorage.Domain;

namespace Personal.BlobStorage.Infrastructure
{
    public interface IBlobFileEventHandler
    {
        Task Handle(BlobFileInfo blobFileInfo);
    }
}
