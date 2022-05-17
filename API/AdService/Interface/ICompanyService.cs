using AdCore.Dto;
using AdCore.Dto.Companies;
using AdCore.Entity;

namespace AdService.Interface
{
    public interface ICompanyService : IBaseService<Company, CompanyDto>
    {
        Task<CompanyDto> CreateAsync(CompanyCreateOrUpdateDto companyCreate);
        Task<string> UploadLogo(string companyId, FileToUpload fileToUpload);
        Task<List<string>> UploadBanner(string companyId, IList<FileToUpload> filesToUpload);
        Task<bool> DeleteBanner(string companyId, string bannerId);
        Task<CompanyDto> UpdateAsync(string id, CompanyCreateOrUpdateDto companyCreate);
    }
}
