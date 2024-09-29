using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Linq.Expressions;
using Personal.BlobStorage.Domain;
using Microsoft.Extensions.Logging;

namespace Personal.BlobStorage.Infrastructure
{
    public class BlobClientUtilityService : IBlobClientUtilityService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly IBlobFileInfoRepository _blobFileInfoRepository;
        private readonly ILogger<BlobClientUtilityService> _logger;
        public BlobClientUtilityService(BlobServiceClient blobServiceClient, IBlobFileInfoRepository blobFileInfoRepository, ILogger<BlobClientUtilityService> logger)
        {
            _containerClient = blobServiceClient.GetBlobContainerClient("container1");
            _blobFileInfoRepository = blobFileInfoRepository;
            _logger = logger;
        }
        public async Task UploadAsync(Stream memoryStream, string fileName)
        {

            var blobClient = _containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                Console.WriteLine("Blob already exits");
            }

            var propeties = await blobClient.GetPropertiesAsync();
            var uploadOptions = new BlobUploadOptions()
            {
                Conditions = new BlobRequestConditions()
                {
                    IfMatch = propeties.Value.ETag
                }
            };

            await blobClient.UploadAsync(memoryStream, uploadOptions);
        }

        public async Task<string> UploadFileAsync(string fileName, string filePath)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["FileName"] = fileName }))
            {
                _logger.LogInformation("Uploding file from repository");

                try
                {
                    _logger.LogInformation("Started uploading file into blob");
                    var blobClient = _containerClient.GetBlobClient(filePath);

                    await blobClient.UploadAsync(fileName);
                    _logger.LogInformation("Started uploaded file into blob");



                    var blobFileInfo = new BlobFileInfo(fileName);
                    _logger.LogInformation("Started updating file information into container");
                    await _blobFileInfoRepository.UpsertBlobFileInfo(blobFileInfo);

                    _logger.LogInformation("Successfully updated file information into container");

                    return blobClient.Uri.AbsoluteUri;
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Exception occured while uploading into blob or updating");
                    throw;
                }
            }
        }

        public async Task UploadBlobFileAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                if (!(await blobClient.ExistsAsync()))
                {
                    return;
                }
                var metaData = new Dictionary<string, string>() { ["FileName"] = fileName };
                var blobFileInfo = new BlobFileInfo(fileName, metaData);
                _logger.LogInformation("Started updating file information into container");
                await _blobFileInfoRepository.UpsertBlobFileInfo(blobFileInfo);
                _logger.LogInformation("Successfully updated file information into container");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Exception occured while upsering file into container");
                throw;
            }
        }

        public async Task<Stream> GetBlobContentInfo(string uri)
        {
            var blobClient = _containerClient.GetBlobClient(uri );
            var blobContent = await blobClient.DownloadContentAsync();
            return blobContent.Value.Content.ToStream();
        }

        //public async Task<Stream> GetBlobContentInfo(string uri)
        //{
        //    var blobItemList = _containerClient.GetBlobsAsync(BlobTraits.All, BlobStates.None, "046C3519AC34E").ToBlockingEnumerable().ToList();
        //    var blobClient = _containerClient.GetBlobClient(blobItemList.First().Name);
        //    var blobContent = await blobClient.DownloadContentAsync();
        //    return blobContent.Value.Content.ToStream();
        //}

        public async Task ArchiveBlobAsync(string blobName, BlobProperties properties)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(blobName);

                if (!await blobClient.ExistsAsync())
                {
                    _logger.LogInformation("Blob don't exist");
                    return;
                }
                var metaData = properties!.Metadata;
                metaData["ProcessStatus"] = "Archived";
                await blobClient.SetMetadataAsync(metaData);

                await blobClient.SetAccessTierAsync(AccessTier.Archive);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Failed to archive the file");
            }
        }
        public async Task<string> GetBlobFileInfo(string blobFileName)
        {
            BlobFileInfo? blobFileInfo = null;
            try
            {
                 blobFileInfo = await _blobFileInfoRepository.GetBlobFileInfoAsync(blobFileName);

                if(blobFileInfo == null || blobFileInfo.MetaData == null)
                {
                    throw new Exception($"failed to retrieve blobInfo with filename {blobFileName}");
                }
                return blobFileInfo.MetaData["fileInfo"];
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "some exception while retrieving the blob file info");
                return string.Empty;
            }
        }
    }
}
    