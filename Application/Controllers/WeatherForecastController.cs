using Microsoft.AspNetCore.Mvc;
using Personal.BlobStorage.Infrastructure;
using System.ComponentModel.DataAnnotations;

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

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBlobClientUtilityService blobClientUtilityService)
        {
            _logger = logger;
            _blobClientUtilityService = blobClientUtilityService;
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

            var num =
                from n in numbersSet
                where n.Count(n => n > 3) > 3
                select n;

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
    }
}
