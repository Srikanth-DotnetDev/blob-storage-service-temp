using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal.BlobStorage.Infrastructure
{
    public interface ILibraryUtilityService
    {
        Task<IEnumerable<LibraryBook>?> GetBookContents(string fileName, string authorName);

    }
}
