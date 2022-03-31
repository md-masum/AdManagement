using Microsoft.AspNetCore.Http;

namespace AdCore.Interface
{
    public interface IFileUploadService
    {
        bool DeleteFile(string key);
        Task<string> UploadFile(string keyPrefix, IFormFile file);
        Task<List<string>> UploadFiles(List<(string keyPrefix, IFormFile file)> uploadFiles);
    }
}