using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Azure.Storage.Blobs;
using Personal.BlobStorage.Domain;
namespace Personal.BlobStorage.Infrastructure
{
        
    public class BlobFileEventHandler : IBlobFileEventHandler
    {
        private readonly ILogger<BlobFileEventHandler> _logger;
        private readonly BlobContainerClient _containerClient;
        //private readonly IStudentAssessmentRawDataHandler _studentAssessmentRawDataHandler;

        public BlobFileEventHandler(ILogger<BlobFileEventHandler> _logger, BlobServiceClient blobServiceClient/*, IStudentAssessmentRawDataHandler studentAssessmentRawDataHandler*/)
        {
            this._logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
            _containerClient = blobServiceClient.GetBlobContainerClient("container1") ?? throw new ArgumentNullException(nameof(blobServiceClient));
            //_studentAssessmentRawDataHandler = studentAssessmentRawDataHandler;
        }
        public async Task<Stream> GetBlobContentInfo(string uri)
        {
            var blobClient = _containerClient.GetBlobClient(uri);
            var blobContent = await blobClient.DownloadContentAsync();
            return blobContent.Value.Content.ToStream();
        }

        public async Task Handle(BlobFileInfo blobFileInfo)
        {
            var fileName = blobFileInfo.BlobFileName;
            _logger.LogInformation("Processing file");
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    var blobClient = _containerClient.GetBlobClient(fileName);
                    await blobClient.DownloadToAsync(memoryStream);
                    memoryStream.Position = 0;

                    if (fileName.ToLower().EndsWith(".csv"))
                    {
                        var csv = "";
                        var line = "";
                        using (var sr = new StreamReader(memoryStream))
                        {
                            var header = sr.ReadLine();
                            while (!sr.EndOfStream)
                            {
                                line = sr.ReadLine();
                                csv = header + "\r\n" + line;
                                Console.WriteLine(csv);
                                //await _studentAssessmentRawDataHandler.Handle(csv, fileName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload file to blob storage.");
                }
            }
        }
    }
}
