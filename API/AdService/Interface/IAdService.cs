using AdCore.Dto.Ads;
using AdCore.Entity;
using Microsoft.AspNetCore.Http;

namespace AdService.Interface
{
    public interface IAdService : IBaseService<Ad, AdDto>
    {
        Task<AdDto> Create(AdToCreateDto adToCreate);
        Task<string> UploadFile(string adId, IFormFile file);
        Task<IList<string>> UploadFiles(string adId, IList<IFormFile> files);
        Task<bool> DeleteFile(string adId, string fileId);
        Task<AdDto> Update(string id, AdToUpdateDto adToUpdate);
    }
}
