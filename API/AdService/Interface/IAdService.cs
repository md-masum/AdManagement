using AdCore.Dto;
using AdCore.Entity;

namespace AdService.Interface
{
    public interface IAdService : IBaseService<Ad, AdDto>
    {
        Task<AdDto> Create(AdToCreateDto adToCreate);
        Task<AdDto> Update(string id, AdToUpdateDto adToUpdate);
    }
}
