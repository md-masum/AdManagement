using AdCore.Dto;
using AdCore.Entity;
using AdCore.Exceptions;
using AdCore.Interface;
using AdRepository.Interface;
using AdService.Base;
using AdService.Interface;
using AutoMapper;

namespace AdService
{
    public class AdService : BaseService<Ad, AdDto>, IAdService
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public AdService(ICosmosDbRepository<Ad> baseRepository, 
            IMapper mapper, 
            ICurrentUserService currentUserService) : base(baseRepository, mapper)
        {
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<AdDto> Create(AdToCreateDto adToCreate)
        {
            var ad = _mapper.Map<Ad>(adToCreate);
            ad.SellerId = _currentUserService.UserId;
            ad.SellerName = _currentUserService.UserName;
            var data = await BaseRepository.AddAsync(ad);

            return _mapper.Map<AdDto>(data);
        }

        public async Task<AdDto> Update(string id, AdToUpdateDto adToUpdate)
        {
            var existingAd = await BaseRepository.GetAsync(id);
            if (existingAd is null)
            {
                throw new NotFoundException("No ad found by provided id");
            }
            var data = await BaseRepository.UpdateAsync(id, _mapper.Map<Ad>(adToUpdate));
            return _mapper.Map<AdDto>(data);
        }
    }
}
