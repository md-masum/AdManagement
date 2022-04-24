using AdCore.Constant;
using AdCore.Dto;
using AdCore.Entity;
using AdCore.Exceptions;
using AdCore.Interface;
using AdRepository.Interface;
using AdService.Base;
using AdService.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace AdService
{
    public class AdService : BaseService<Ad, AdDto>, IAdService
    {
        private readonly IMapper _mapper;
        private readonly IFileUploadService _fileUploadService;
        private readonly ICacheService _cacheService;
        private readonly ICurrentUserService _currentUserService;

        public AdService(ICosmosDbRepository<Ad> baseRepository, 
            IMapper mapper, 
            IFileUploadService fileUploadService,
            ICacheService cacheService,
            ICurrentUserService currentUserService) : base(baseRepository, mapper)
        {
            _mapper = mapper;
            _fileUploadService = fileUploadService;
            _cacheService = cacheService;
            _currentUserService = currentUserService;
        }
        public override async Task<IList<AdDto>> GetAllAsync()
        {
            var cacheData = _cacheService.Get<List<AdDto>>(CacheKey.AdList);
            if (cacheData is not null) return cacheData;

            var entityData = await BaseRepository.GetAllAsync();

            if (entityData is null || !entityData.Any()) return default;

            var dataToReturn = await MapAdServices(entityData);
            _cacheService.Set(CacheKey.AdList, dataToReturn);

            return dataToReturn;
        }

        public override async Task<AdDto> GetByIdAsync(string id)
        {
            var entityData = await BaseRepository.GetAsync(id);
            if (entityData == null) throw new NotFoundException("Ad not found");

            var dataToReturn = await MapAdService(entityData);

            return dataToReturn;
        }

        public async Task<AdDto> Create(AdToCreateDto adToCreate)
        {
            var ad = _mapper.Map<Ad>(adToCreate);
            ad.SellerId = _currentUserService.UserId;
            ad.SellerName = _currentUserService.UserName;
            var data = await BaseRepository.AddAsync(ad);
            _cacheService.Remove(CacheKey.AdList);

            return _mapper.Map<AdDto>(data);
        }

        public async Task<string> UploadFile(string adId, IFormFile file)
        {
            var ad = await BaseRepository.GetAsync(adId);
            if (ad is null) throw new NotFoundException("No ad found by provided id");

            var uploadedFile = await _fileUploadService.UploadFileCloud(file);

            ad.FileId ??= new List<string>();
            ad.FileId.Add(uploadedFile.Id);

            await BaseRepository.UpdateAsync(adId, ad);
            _cacheService.Remove(CacheKey.AdList);
            return uploadedFile.Url;
        }

        public async Task<IList<string>> UploadFiles(string adId, IList<IFormFile> files)
        {
            var filesPath = new List<string>();
            var ad = await BaseRepository.GetAsync(adId);
            if (ad is null) throw new NotFoundException("No ad found by provided id");

            foreach (var file in files)
            {
                var uploadedFile = await _fileUploadService.UploadFileCloud(file);
                filesPath.Add(uploadedFile.Url);

                ad.FileId ??= new List<string>();
                ad.FileId.Add(uploadedFile.Id);
            }

            await BaseRepository.UpdateAsync(adId, ad);
            _cacheService.Remove(CacheKey.AdList);
            return filesPath;
        }

        public async Task<bool> DeleteFile(string adId, string fileId)
        {
            var ad = await BaseRepository.GetAsync(adId);
            if (ad is null) throw new NotFoundException("No ad found by provided id");
            await _fileUploadService.DeleteFileCloud(fileId);
            ad.FileId.RemoveAll(f => f == fileId);
            await BaseRepository.UpdateAsync(adId, ad);
            _cacheService.Remove(CacheKey.AdList);
            return true;
        }

        public async Task<AdDto> Update(string id, AdToUpdateDto adToUpdate)
        {
            var existingAd = await BaseRepository.GetAsync(id);
            if (existingAd is null)
            {
                throw new NotFoundException("No ad found by provided id");
            }
            var data = await BaseRepository.UpdateAsync(id, _mapper.Map<Ad>(adToUpdate));
            _cacheService.Remove(CacheKey.AdList);
            return _mapper.Map<AdDto>(data);
        }

        private async Task<AdDto> MapAdService(Ad entityData)
        {
            var dataToReturn = _mapper.Map<AdDto>(entityData);
            if (entityData.FileId is null || entityData.FileId.Count <= 0) return dataToReturn;
            var files = await _fileUploadService.GetAllAsync(c => entityData.FileId.Contains(c.Id));
            dataToReturn.AdFiles ??= new List<AdFileDto>();
            dataToReturn.AdFiles.AddRange(files);
            return dataToReturn;
        }

        private async Task<List<AdDto>> MapAdServices(IList<Ad> adList)
        {
            var dataToReturn = new List<AdDto>();
            foreach (var entityData in adList)        
            {
                var adDto = _mapper.Map<AdDto>(entityData);
                if (entityData.FileId is not null && entityData.FileId.Count > 0)
                {
                    var files = await _fileUploadService.GetAllAsync(c => entityData.FileId.Contains(c.Id));
                    adDto.AdFiles ??= new List<AdFileDto>();
                    adDto.AdFiles.AddRange(files);
                }
                dataToReturn.Add(adDto);
            }
            return dataToReturn;
        }
    }
}
