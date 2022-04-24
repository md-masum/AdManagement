using AdCore.Dto;
using AdCore.Entity;
using Microsoft.AspNetCore.Http;

namespace AdService.Interface
{
    public interface IFileUploadService : IBaseService<AdFile, AdFileDto>
    {
        Task<bool> DeleteFileDisk(string key);
        Task<bool> DeleteFileCloud(string key);
        Task<AdFileDto> UploadFileDisk(IFormFile file, string keyPrefix = null);
        Task<AdFileDto> UploadFileCloud(IFormFile file, string keyPrefix = null);

        Task<List<AdFileDto>> UploadFilesDisk(
            List<(IFormFile file, string keyPrefix)> uploadFiles);

        Task<List<AdFileDto>> UploadFilesCloud(
            List<(IFormFile file, string keyPrefix)> uploadFiles);
    }
}