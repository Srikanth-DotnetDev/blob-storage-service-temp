using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Azure.Storage.Blobs;
using Personal.BlobStorage.Domain;
using Azure.Storage.Blobs.Models;
namespace Personal.BlobStorage.Infrastructure
{
        
    public class BlobFileEventHandler : IBlobFileEventHandler
    {
        private readonly ILogger<BlobFileEventHandler> _logger;
        private readonly IBlobClientUtilityService _blobClientUtilityService;
        //private readonly IStudentAssessmentRawDataHandler _studentAssessmentRawDataHandler;

        public BlobFileEventHandler(ILogger<BlobFileEventHandler> _logger, IBlobClientUtilityService blobClientUtilityService/*, IStudentAssessmentRawDataHandler studentAssessmentRawDataHandler*/)
        {
            this._logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
            //_studentAssessmentRawDataHandler = studentAssessmentRawDataHandler;
            _blobClientUtilityService = blobClientUtilityService;
        }

        public async Task Handle(BlobFileInfo blobFileInfo)
        {
            _logger.LogInformation("Processing file");
            try
            {
                var blobFileName = blobFileInfo.BlobFileName;

                string? header, allLines;
                using (var stream = await _blobClientUtilityService.GetBlobContentInfo(blobFileName))
                {
                    if (stream is null)
                    {
                        _logger.LogWarning("Stream is null, exiting the method");
                        return;
                    }
                    using var sr = new StreamReader(stream);
                    header = sr.ReadLine();

                    if (header is null)
                    {
                        _logger.LogWarning("The file is empty, exiting the method");
                        return;
                    }
                    allLines = await sr.ReadToEndAsync();
                }

                var rows = allLines.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select((line, index) => new { Line = header + "\r\n" + line, RowNumber = index + 1 })
                                   .ToArray();


                rows.ToList().ForEach(row =>
                {
                    //var result = await _dataHandler.Handle(row.Line, blobFileName, row.RowNumber);
                    Console.WriteLine(row);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file to blob storage.");
            }
        }
    }
}
