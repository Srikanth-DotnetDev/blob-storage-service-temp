using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal.BlobStorage.Infrastructure
{
    public interface IBlobClientUtilityService
    {
        Task UploadAsync(Stream memoryStream, string fileName);
        Task<string> UploadFileAsync(string fileName, string filePath);
        Task<Stream> GetBlobContentInfo(string uri);
        Task UploadBlobFileAsync(string fileName);
        Task ArchiveBlobAsync(string blobName, BlobProperties properties);
        Task<string> GetBlobFileInfo(string blobFileName);
    }
}
