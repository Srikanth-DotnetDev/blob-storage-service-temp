using Personal.BlobStorage.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal.BlobStorage.Infrastructure
{
    public interface IBlobFileInfoRepository
    {
        Task<BlobFileInfo?> GetBlobFileInfoAsync(string blobFileName);
        Task UpsertBlobFileInfo(BlobFileInfo blobFileInfo);
    }
}
