using AdCore.Dto;
using AdCore.Entity;
using AdCore.Enums;
using AdCore.Exceptions;
using AdCore.Extensions;
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
        private readonly ICurrentUserService _currentUserService;

        public AdService(ICosmosDbRepository<Ad> baseRepository, 
            IMapper mapper, 
            IFileUploadService fileUploadService,
            ICurrentUserService currentUserService) : base(baseRepository, mapper)
        {
            _mapper = mapper;
            _fileUploadService = fileUploadService;
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

        public async Task<string> UploadFile(string adId, IFormFile file)
        {
            var ad = await BaseRepository.GetAsync(adId);
            if (ad is null) throw new NotFoundException("No ad found by provided id");
            var filePath = await _fileUploadService.UploadFileDisk(file);

            switch (file.FileName.GetFileExtension().GetFileType())
            {
                case FileTypes.Document:
                    ad.DocumentUrl ??= new List<string>();
                    ad.DocumentUrl.Add(filePath);
                    break;
                case FileTypes.Image:
                    ad.ImageUrl ??= new List<string>();
                    ad.ImageUrl.Add(filePath);
                    break;
                case FileTypes.Pdf:
                    ad.PdfUrl ??= new List<string>();
                    ad.PdfUrl.Add(filePath);
                    break;
                case FileTypes.Video:
                    ad.VideoUrl ??= new List<string>();
                    ad.VideoUrl.Add(filePath);
                    break;
                default: throw new CustomException("Invalid file format");
            }

            await BaseRepository.UpdateAsync(adId, ad);
            return filePath;
        }

        public async Task<IList<string>> UploadFiles(string adId, IList<IFormFile> files)
        {
            var filesPath = new List<string>();
            var ad = await BaseRepository.GetAsync(adId);
            if (ad is null) throw new NotFoundException("No ad found by provided id");

            foreach (var file in files)
            {
                var filePath = await _fileUploadService.UploadFileDisk(file);
                filesPath.Add(filePath);

                switch (file.FileName.GetFileExtension().GetFileType())
                {
                    case FileTypes.Document:
                        ad.DocumentUrl ??= new List<string>();
                        ad.DocumentUrl.Add(filePath);
                        break;
                    case FileTypes.Image:
                        ad.ImageUrl ??= new List<string>();
                        ad.ImageUrl.Add(filePath);
                        break;
                    case FileTypes.Pdf:
                        ad.PdfUrl ??= new List<string>();
                        ad.PdfUrl.Add(filePath);
                        break;
                    case FileTypes.Video:
                        ad.VideoUrl ??= new List<string>();
                        ad.VideoUrl.Add(filePath);
                        break;
                    default: throw new CustomException("Invalid file format");
                }
            }

            await BaseRepository.UpdateAsync(adId, ad);
            return filesPath;
        }

        public async Task<bool> DeleteFile(string adId, string fileName)
        {
            var ad = await BaseRepository.GetAsync(adId);
            if (ad is null) throw new NotFoundException("No ad found by provided id");
            _fileUploadService.DeleteFileDisk(fileName);

            switch (fileName.GetFileExtension().GetFileType())
            {
                case FileTypes.Document:
                    ad.DocumentUrl.Remove(fileName);
                    break;
                case FileTypes.Image:
                    ad.ImageUrl.Remove(fileName);
                    break;
                case FileTypes.Pdf:
                    ad.PdfUrl.Remove(fileName);
                    break;
                case FileTypes.Video:
                    ad.VideoUrl.Remove(fileName);
                    break;
                default: throw new CustomException("Invalid file format");
            }

            await BaseRepository.UpdateAsync(adId, ad);
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
            return _mapper.Map<AdDto>(data);
        }
    }
}
