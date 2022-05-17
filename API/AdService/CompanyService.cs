using AdCore.Dto;
using AdCore.Dto.Companies;
using AdCore.Entity;
using AdCore.Enums;
using AdCore.Exceptions;
using AdCore.Interface;
using AdRepository.Interface;
using AdService.Base;
using AdService.Interface;
using AutoMapper;

namespace AdService
{
    public class CompanyService : BaseService<Company, CompanyDto>, ICompanyService
    {
        private readonly ICosmosDbRepository<Ad> _adRepository;
        private readonly ICosmosDbRepository<User> _useRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public CompanyService(ICosmosDbRepository<Company> baseRepository,
            ICosmosDbRepository<Ad> adRepository,
            ICosmosDbRepository<User> useRepository,
            ICurrentUserService currentUserService,
            IFileUploadService fileUploadService,
            IMapper mapper) : base(baseRepository, mapper)
        {
            _adRepository = adRepository;
            _useRepository = useRepository;
            _currentUserService = currentUserService;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }

        public async Task<CompanyDto> CreateAsync(CompanyCreateOrUpdateDto companyCreate)
        {
            if (_currentUserService.Roles != Roles.Seller) throw new AuthException("Unauthorize access");
            if (await _currentUserService.HasAssociateCompany())
                throw new CustomException("Seller already has company attached");
            
            var user = await _currentUserService.CurrentLoggedInUser();

            var entity = _mapper.Map<Company>(companyCreate);
            entity.Sellers = new List<string> {user.Id}; 
            entity.MasterSellerAdId = _currentUserService.UserId;
            await BaseRepository.AddAsync(entity);

            var userEntity = _mapper.Map<User>(user);
            userEntity.CompanyId = entity.Id;
            await _useRepository.UpdateAsync(user.Id, userEntity);

            return _mapper.Map<CompanyDto>(entity);
        }

        public async Task<CompanyDto> UpdateAsync(string id, CompanyCreateOrUpdateDto companyCreate)
        {
            var companyToUpdate = _mapper.Map<CompanyDto>(companyCreate);
            var updatedCompany = await UpdateAsync(id, companyToUpdate);
            return updatedCompany;
        }

        public override async Task<bool> RemoveAsync(string id)
        {
            if (!await BaseRepository.DeleteAsync(id)) return false;
            var user = await _currentUserService.CurrentLoggedInUser();
            var userEntity = _mapper.Map<User>(user);
            userEntity.CompanyId = string.Empty;
            await _useRepository.UpdateAsync(user.Id, userEntity);
            return true;
        }

        public async Task<string> UploadLogo(string companyId, FileToUpload fileToUpload)
        {
            if (_currentUserService.Roles != Roles.Seller) throw new AuthException("Insufficient permission");
            if (!await _currentUserService.HasAssociateCompany())
                throw new CustomException("Seller has no company attached");

            var company = await GetByIdAsync(companyId);

            if (company.MasterSellerAdId != _currentUserService.UserId) 
                throw new AuthException("Insufficient permission");

            if (company.LogoId is not null) await _fileUploadService.DeleteFileCloud(company.LogoId);

            var uploadedFile = await _fileUploadService.UploadFileCloud(fileToUpload.File);

            company.LogoUrl = uploadedFile.Url;
            company.LogoId = uploadedFile.Id;
            await UpdateAsync(company.Id, company);

            return uploadedFile.Url;
        }

        public async Task<List<string>> UploadBanner(string companyId, IList<FileToUpload> filesToUpload)
        {
            if (_currentUserService.Roles != Roles.Seller) throw new AuthException("Insufficient permission");
            if (!await _currentUserService.HasAssociateCompany())
                throw new CustomException("Seller has no company attached");

            var company = await GetByIdAsync(companyId);

            if (!company.Sellers.Contains(_currentUserService.UserId))
                throw new AuthException("Insufficient permission");

            var uploadedFile = await _fileUploadService.UploadFilesCloud(filesToUpload.Select(s => s.File).ToList());

            company.Banners = uploadedFile.ToDictionary(k => k.Id, e => e.Url);
            await UpdateAsync(company.Id, company);

            return uploadedFile.Select(s => s.Url).ToList();
        }

        public async Task<bool> DeleteBanner(string companyId, string bannerId)
        {
            var company = await GetByIdAsync(companyId);
            if (!company.Banners.ContainsKey(bannerId)) return false;

            await _fileUploadService.DeleteFileCloud(bannerId);
            company.Banners.Remove(bannerId);
            await UpdateAsync(companyId, company);
            return true;
        }
    }
}
