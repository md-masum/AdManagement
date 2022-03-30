using AdCore.Dto;
using AdService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, Seller")]
    [ApiController]
    public class AdController : ControllerBase
    {
        private readonly IAdService _adService;

        public AdController(IAdService adService)
        {
            _adService = adService;
        }

        [HttpGet]
        public async Task<ActionResult<IList<AdToReturnDto>>> Get()
        {
            return Ok(await _adService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdToReturnDto>> GetById(string id)
        {
            return Ok(await _adService.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult<AdToReturnDto>> Create([FromForm]AdToCreateDto adToCreate)
        {
            return Ok(await _adService.Create(adToCreate));
        }

        [HttpPut("{id}")]
        public ActionResult<AdToReturnDto> Update(string id, AdToUpdateDto adToUpdate)
        {
            return Ok(_adService.Update(id, adToUpdate));
        }

        [HttpDelete("{id}")]
        public ActionResult<bool> Delete(string id)
        {
            return Ok(_adService.Delete(id));
        }
    }
}
