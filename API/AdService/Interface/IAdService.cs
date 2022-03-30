using AdCore.Dto;
using AdCore.Entity;

namespace AdService.Interface
{
    public interface IAdService
    {
        Task<IList<AdToReturnDto>> GetAll();
        Task<AdToReturnDto> GetById(string id);
        Task<AdToReturnDto> Create(AdToCreateDto adToCreate);
        Task<AdToReturnDto> Update(string id, AdToUpdateDto adToUpdate);
        Task<bool> Delete(string id);
    }
}
