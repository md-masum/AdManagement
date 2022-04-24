using AdCore.Dto;
using AdCore.Dto.Ads;
using AdCore.Response;
using AdService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Seller")]
    [ApiController]
    public class AdController : ControllerBase
    {
        private readonly IAdService _adService;

        public AdController(IAdService adService)
        {
            _adService = adService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IList<AdDto>>>> Get()
        {
            return Ok(new ApiResponse<IList<AdDto>>(await _adService.GetAllAsync()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AdDto>>> GetById(string id)
        {
            return Ok(new ApiResponse<AdDto>(await _adService.GetByIdAsync(id)));
        }

        [HttpPost]
        public async Task<ActionResult<AdDto>> Create([FromForm]AdToCreateDto adToCreate)
        {
            return Ok(new ApiResponse<AdDto>(await _adService.Create(adToCreate)));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AdDto>>> Update(string id, AdToUpdateDto adToUpdate)
        {
            return Ok(new ApiResponse<AdDto>(await _adService.Update(id, adToUpdate)));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            return Ok(new ApiResponse<bool>(await _adService.RemoveAsync(id)));
        }

        [HttpPut("uploadFiles/{adId}")]
        public async Task<ActionResult<IList<string>>> UploadFiles(string adId, [FromForm] List<FileToUpload> files)
        {
            return Ok(new ApiResponse<IList<string>>(await _adService.UploadFiles(adId, files.Select(s => s.File).ToList())));
        }

        [HttpPut("uploadFile/{adId}")]
        public async Task<ActionResult<IList<string>>> UploadFIle(string adId, [FromForm] FileToUpload file)
        {
            return Ok(new ApiResponse<IList<string>>(await _adService.UploadFile(adId, file.File)));
        }

        [HttpDelete("deleteFile/{adId}")]
        public async Task<ActionResult<IList<string>>> DeleteFIle(string adId, string fileName)
        {
            return Ok(new ApiResponse<bool>(await _adService.DeleteFile(adId, fileName)));
        }
    }
}
