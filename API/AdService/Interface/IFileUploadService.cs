using AdCore.Dto.Ads;
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
            List<IFormFile> uploadFiles);

        Task<List<AdFileDto>> UploadFilesCloud(
            List<IFormFile> uploadFiles);
    }
}