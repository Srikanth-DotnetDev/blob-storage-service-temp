using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using System.Xml;

namespace Personal.BlobStorage.Infrastructure
{
    public class LibraryUtilityService : ILibraryUtilityService
    {
        private readonly ILogger<LibraryUtilityService> _logger;
        private readonly IBlobClientUtilityService _blobClientUtilityService;
        private readonly IOptions<NameBasedUUIDOptions> _options;

        public LibraryUtilityService(ILogger<LibraryUtilityService> logger, IBlobClientUtilityService blobClientUtilityService, IOptions<NameBasedUUIDOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _blobClientUtilityService = blobClientUtilityService ?? throw new ArgumentNullException(nameof(blobClientUtilityService));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IEnumerable<LibraryBook>?> GetBookContents(string fileName, string authorName)
        {
            try
            {

                string strName = _options.Value.EnvironmentId;
                var fileContent = await _blobClientUtilityService.GetBlobFileInfo(fileName);

                if(fileContent == null)
                {
                    throw new Exception($"file contents are empty");
                }

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(fileContent);

                var libraryBooks = JsonConvert.DeserializeObject<Library>(JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true))!;

                return  from book in libraryBooks.Books
                        where book.Author == authorName
                        select book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retreiving the book contents");
                return null;
            }
        }

        #region Common Method 
        private static string ConstructRequestXml(object requestObject)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(requestObject.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, requestObject);

                return textWriter.ToString();
            }
        }

        private static object? ConstructResponseObject(Type type, string responseObject)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type);

            using (StringReader reader = new StringReader(responseObject))
            {
                return xmlSerializer.Deserialize(reader);
            }
        }
        #endregion
    }
}
