using AdCore.Dto;
using AdCore.Entity;
using AdCore.Interface;
using AdRepository.Interface;
using AdService.Interface;
using AutoMapper;

namespace AdService
{
    public class AdService : IAdService
    {
        private readonly ICosmosDbRepository<Ad> _adRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public AdService(ICosmosDbRepository<Ad> adRepository, 
            IMapper mapper, 
            ICurrentUserService currentUserService)
        {
            _adRepository = adRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<IList<AdToReturnDto>> GetAll()
        {
            var data = await _adRepository.GetAllAsync();
            return _mapper.Map<IList<AdToReturnDto>>(data);
        }

        public async Task<AdToReturnDto> GetById(string id)
        {
            var data = await _adRepository.GetAsync(id);
            return _mapper.Map<AdToReturnDto>(data);
        }

        public async Task<AdToReturnDto> Create(AdToCreateDto adToCreate)
        {
            var ad = _mapper.Map<Ad>(adToCreate);
            ad.SellerId = _currentUserService.UserId;
            ad.SellerName = _currentUserService.UserName;

            var data = await _adRepository.AddAsync(ad);

            return _mapper.Map<AdToReturnDto>(data);
        }

        public async Task<AdToReturnDto> Update(string id, AdToUpdateDto adToUpdate)
        {
            var existingAd = await _adRepository.GetAsync(id);
            if (existingAd is null)
            {
                throw new Exception("No ad found by provided id");
            }
            var data = await _adRepository.UpdateAsync(id, _mapper.Map<Ad>(adToUpdate));
            return _mapper.Map<AdToReturnDto>(data);
        }

        public async Task<bool> Delete(string id)
        {
            return await _adRepository.DeleteAsync(id);
        }
    }
}
