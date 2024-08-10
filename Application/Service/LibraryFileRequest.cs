
namespace Personal.BlobStorage.Application
{
    public class LibraryFileRequest
    {
        public LibraryFileRequest(string fileName, string authorName)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
        }

        public string FileName { get; set; }
        public string AuthorName { get; set; }
    }
}
