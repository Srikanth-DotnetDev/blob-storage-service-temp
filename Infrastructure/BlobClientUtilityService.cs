﻿using Azure.Storage.Blobs.Models;
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

    public interface IBlobClientUtilityService
    {
        Task UploadAsync(Stream memoryStream, string fileName);
        Task<string> UploadFileAsync(string fileName, string filePath);
        Task<Stream> GetBlobContentInfo(string uri);
    }
    public class BlobClientUtilityService : IBlobClientUtilityService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private readonly IBlobFileInfoRepository _blobFileInfoRepository;
        private readonly ILogger<BlobClientUtilityService> _logger;
        public BlobClientUtilityService(BlobServiceClient blobServiceClient, IBlobFileInfoRepository blobFileInfoRepository, ILogger<BlobClientUtilityService> logger) 
        {
            _blobServiceClient = blobServiceClient;
            _containerClient = blobServiceClient.GetBlobContainerClient("container1");
            _blobFileInfoRepository = blobFileInfoRepository;
            _logger = logger;
        }
        private readonly string _blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=blobsftpdemo3;AccountKey=6t3gR4YDX/3rad17BOT3ix0awaeD9xZBRoSix3CkfZZRelO2xHhNK+1Itt2twqb8xD2j5apNZyM3+AStYEaJdw==;EndpointSuffix=core.windows.net";
        private readonly string _blobContainerName = "container1";
        public async Task UploadAsync(Stream memoryStream, string fileName)
        {
            var blobClient = new BlobClient(_blobStorageConnectionString, _blobContainerName, Path.GetFileName(fileName));

            //var blobClient = _containerClient.GetBlobClient(fileName);

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
            using (_logger.BeginScope(new Dictionary<string, object> { ["FileName"] = fileName}) )
            {
                _logger.LogInformation("Uploding file from repository");

                try
                {
                    _logger.LogInformation("Started uploading file into blob");
                    var blobClient = _containerClient.GetBlobClient(fileName);
                    await blobClient.UploadAsync(filePath);
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
            }        }

        public async Task<Stream> GetBlobContentInfo(string uri)
        {
            var blobClient = _containerClient.GetBlobClient(uri );
            var blobContent = await blobClient.DownloadContentAsync();
            return blobContent.Value.Content.ToStream();
        }
    }
}
