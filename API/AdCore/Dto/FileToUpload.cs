using Microsoft.AspNetCore.Http;

namespace AdCore.Dto
{
    public class FileToUpload
    {
        public IFormFile File { get; set; }
    }
}
