using Microsoft.AspNetCore.Http;

namespace AdCore.Interface
{
    public interface IFileUploadService
    {
        bool DeleteFileDisk(string key);
        Task<bool> DeleteFileCloud(string key);
        Task<string> UploadFileDisk(IFormFile file, string keyPrefix = null);
        Task<string> UploadFileCloud(IFormFile file, string keyPrefix = null);

        Task<List<string>> UploadFilesDisk(
            List<(IFormFile file, string keyPrefix)> uploadFiles);

        Task<List<string>> UploadFilesCloud(
            List<(IFormFile file, string keyPrefix)> uploadFiles);
    }
}