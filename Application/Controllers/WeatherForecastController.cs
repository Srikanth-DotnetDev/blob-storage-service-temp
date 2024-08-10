using Microsoft.AspNetCore.Mvc;
using Personal.BlobStorage.Application;
using Personal.BlobStorage.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IBlobClientUtilityService _blobClientUtilityService;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly ILibraryUtilityService _libraryUtilityService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBlobClientUtilityService blobClientUtilityService, ILibraryUtilityService libraryUtilityService)
        {
            _logger = logger;
            _blobClientUtilityService = blobClientUtilityService;
            _libraryUtilityService = libraryUtilityService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
             var sum = (int a, int b) => a + b;

             Console.WriteLine(sum(1, 4));

             var numbersSet = new List<int[]>
                                            {
                                                new[] {1,2,3,4},
                                                new[] {5,6,7,8},
                                                new[] {9,10,11,12}
                                            };

            var num = from n in numbersSet
                      select n.Sum(); 

            foreach (var item in num)
            {
                Console.WriteLine(string.Join(",", item));
            }


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();


        }
       
        [HttpPost]
        public async Task<IActionResult> UploadFile(FileUploadModel model)
        {
            if (model == null || model.fileName == null || model.filePath == null)
            {
                _logger.LogInformation("File request doesnot meet standard");
                return BadRequest();
            }
            _logger.LogInformation("Uplading the file to blob storage");
            try
            {
                var result = await _blobClientUtilityService.UploadFileAsync(model.fileName, model.filePath);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured while uploading to blob storage");
                return Problem();
            }
        }
        [HttpGet("GetBlob")]
        public async Task<IActionResult> GetBlobContentInfo(string uri)
        {
            if (uri == null)
            {
                _logger.LogInformation("Request doesnot meet standard");
                return BadRequest();
            }
            _logger.LogInformation("Retrieving the blob details");

            try
            {
                var result = await _blobClientUtilityService.GetBlobContentInfo(uri);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured while retrieving to blob storage");
                return Problem();
            }
        }
        [HttpPost("UploadBlobFile")]
        public async Task<IActionResult> UploadBlobFile(string uri)
        {
            if (uri == null)
            {
                _logger.LogInformation("Request doesnot meet standard");
                return BadRequest();
            }
            _logger.LogInformation("Retrieving the blob details");

            try
            {
                await _blobClientUtilityService.UploadBlobFileAsync(uri);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured while retrieving to blob storage");
                return Problem();
            }
        }
        [HttpPost("GetLibraryBooks")]
        public async Task<IActionResult> GetLibraryBooks([FromBody]LibraryFileRequest request)
        {
            if(request == null || request.FileName == null || request.AuthorName == null)
            {
                _logger.LogInformation("Request doesnot meet standard");
                return BadRequest(nameof(request));
            }
            _logger.LogInformation("Retrieving the LibraryBook details");
            try
            {
                var bookInfo = await _libraryUtilityService.GetBookContents(request.FileName, request.AuthorName);
                return Ok(bookInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured while retrieving to blob storage");
                return Problem();
            }
        }
    }
}
