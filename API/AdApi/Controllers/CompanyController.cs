using AdCore.Dto;
using AdCore.Dto.Companies;
using AdCore.Response;
using AdService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AdApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<ActionResult<IList<CompanyDto>>> GetAll()
        {
            return Ok(new ApiResponse<IList<CompanyDto>>(await _companyService.GetAllAsync()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> Get(string id)
        {
            return Ok(new ApiResponse<CompanyDto>(await _companyService.GetByIdAsync(id)));
        }

        [HttpPost]
        public async Task<ActionResult<CompanyDto>> Create([FromBody] CompanyCreateOrUpdateDto companyCreate)
        {
            return Ok(new ApiResponse<CompanyDto>(await _companyService.CreateAsync(companyCreate)));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CompanyDto>> Update(string id, [FromBody] CompanyCreateOrUpdateDto companyUpdate)
        {
            return Ok(new ApiResponse<CompanyDto>(await _companyService.UpdateAsync(id, companyUpdate)));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(string id)
        {
            await _companyService.RemoveAsync(id);
            return Ok(new ApiResponse<bool>(await _companyService.RemoveAsync(id)));
        }

        [HttpPost("logo/{companyId}")]
        public async Task<ActionResult<string>> UploadLogo(string companyId, [FromForm] FileToUpload logo)
        {
            return Ok(new ApiResponse<string>(await _companyService.UploadLogo(companyId, logo)));
        }

        [HttpPost("banner/{companyId}")]
        public async Task<ActionResult<List<string>>> UploadBanner(string companyId, [FromForm] List<FileToUpload> banner)
        {
            return Ok(new ApiResponse<List<string>>(await _companyService.UploadBanner(companyId, banner)));
        }

        [HttpDelete("banner/{bannerId}/company/{companyId}")]
        public async Task<ActionResult<bool>> DeleteBanner(string bannerId, string companyId)
        {
            return Ok(new ApiResponse<bool>(await _companyService.DeleteBanner(bannerId, companyId)));
        }
    }
}
