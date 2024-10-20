using Microsoft.AspNetCore.Mvc;
using Personal.BlobStorage.Application;
using Personal.BlobStorage.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Personal.BlobStorage.Domain;
using Domain;

namespace Personal.BlobStorage.Application
{
    [ApiController]
    [Route("[controller]")]
    public class BlobStorageController : ControllerBase
    {
        private readonly IBlobClientUtilityService _blobClientUtilityService;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<BlobStorageController> _logger;

        private readonly ILibraryUtilityService _libraryUtilityService;

        private readonly IProductFileRepository _productFileRepository;

        //private readonly IFeatureManager _featureManager;
        //private readonly IConfigurationRefresher _refresher;
        //private readonly IOptions<NameBasedUUIDOptions> _options;

        public BlobStorageController(ILogger<BlobStorageController> logger, IBlobClientUtilityService blobClientUtilityService, 
            ILibraryUtilityService libraryUtilityService, IProductFileRepository productFileRepository/*, IFeatureManager featureManager,*/
            /*IConfigurationRefresher refresher, IOptions<NameBasedUUIDOptions> options*/)
        {
            _logger = logger;
            _blobClientUtilityService = blobClientUtilityService;
            _libraryUtilityService = libraryUtilityService;
            _productFileRepository = productFileRepository; 
            //_featureManager = featureManager;
            //_refresher = refresher;
            //_options = options;
        }


        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            //var str = System.Net.HttpStatusCode.TooManyRequests.ToString();
            //Console.WriteLine(str + "" + str.GetType());
            // var sum = (int a, int b) => a + b;

            // Console.WriteLine(sum(1, 4));

            // var numbersSet = new List<int[]>
            //                                {
            //                                    new[] {1,2,3,4},
            //                                    new[] {5,6,7,8},
            //                                    new[] {9,10,11,12}
            //                                };

            //var num = from n in numbersSet
            //          select n.Sum();

            //#region generate-range
            //var numbers = from n in Enumerable.Range(100, 50)
            //              select (Number: n, OddEven: n % 2 == 0 ? "Even" : "Odd");


            //foreach (var n in numbers)
            //{
            //    Console.WriteLine("The number {0} is {1}.", n.Number, n.OddEven);
            //}
            //#endregion

            //#region generate-repeat
            //var numbers1 = Enumerable.Repeat(7, 10);

            //foreach (var n in numbers1)
            //{
            //    Console.WriteLine(n);
            //}
            //#endregion

            //foreach (var item in num)
            //{
            //    Console.WriteLine(string.Join(",", item));
            //} 

            var studentId = "1232324";

            var studentLocalId = "";

            var leaNumber = "";

            //var stu =  studentId switch
            //{
            //    _ when !string.IsNullOrEmpty(studentId) => new LegacyStatewideStudentIdentifier(studentId).ToString(),
            //    _ when !string.IsNullOrEmpty(studentLocalId) => new LeaStudentIdentifier(new LeaNumber(leaNumber), studentLocalId).ToString(),
            //    _ => null
            //};

            var n = new StudentRecordLocator($"{studentId}@ssid", $"{studentLocalId}@lea{leaNumber}").ToString();

            List<int> list1 = [1,2, 3 ];

            list1[1] = 2;



            int[] numbers = { 5, 10, 8, 3, 6, 12 };

            //Query syntax:
            IEnumerable<int> numQuery1 =
                from num in numbers
                where num % 2 == 0
                orderby num
                select num;

            //Method syntax:
            IEnumerable<int> numQuery2 = numbers.Where(num => num % 2 == 0).OrderByDescending(n => n);

            foreach (int i in numQuery1)
            {
                Console.Write(i + " ");
            }

            //string str = "srikanth";
            //string str2 = "kan";
            //Console.WriteLine(str.IndexOf(str2));
            //Console.WriteLine(str.GetHashCode());
            //Console.WriteLine(System.Environment.NewLine);
            //foreach (int i in numQuery2)
            //{
            //    Console.Write(i + " ");
            //}


            //var coordinates = new Coords(1, 2);
            //Console.WriteLine(coordinates);
            ////await _refresher.TryRefreshAsync(new CancellationToken());
            ////if (!await _featureManager.IsEnabledAsync("rise-assessments-service.rise-ftp-processor"))
            ////{
            ////    //await _productFileRepository.UpsertItemAsync();
            ////    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            ////    {
            ////        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),

            ////        TemperatureC = Random.Shared.Next(-20, 55),
            ////        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            ////    })
            ////    .ToArray();
            ////}


            //List<string> names = ["srikanth", "Kranthi", "srikanth", "Chandhu", "srikanth"];
            //names = names.Distinct().ToList();  

            //foreach (var item in names.Distinct())
            //{
            //    Console.WriteLine(item);
            //}

            return null!;

            ////var a = new []{1,2,3,4,5};
            ////foreach (var item in a)
            ////{
            ////    var evenOdd = item %2 == 0 ? "Even" : "Odd";
            ////    Console.WriteLine($"Number is {evenOdd}");
            ////}
        }
       
        [HttpPost]
        public async Task<IActionResult> UploadFile(FileUploadModel model)
        {
            if (model == null || model.FileName == null || model.FilePath == null)
            {
                _logger.LogInformation("File request doesnot meet standard");
                return BadRequest();
            }
            _logger.LogInformation("Uplading the file to blob storage");
            try
            {
                var result = await _blobClientUtilityService.UploadFileAsync(model.FileName, model.FilePath);
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
