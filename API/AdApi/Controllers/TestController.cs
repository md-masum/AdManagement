using AdCore.Dto;
using AdCore.Entity;
using AdCore.Response;
using AdService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AdApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IBaseService<Test, TestDto> _testService;

        public TestController(ILogger<TestController> logger,
            IBaseService<Test, TestDto> testService
            )
        {
            _logger = logger;
            _testService = testService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TestDto>>>> Get()
        {
            var testList = await _testService.GetAllAsync();
            return Ok(new ApiResponse<IList<TestDto>>(testList));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TestDto>>> Get(string id)
        {
            return Ok(await _testService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TestDto>>> Post(TestDto test)
        {
            var createdTest = await _testService.AddAsync(test);
            return CreatedAtAction(nameof(Get), createdTest.Id, new ApiResponse<TestDto>(createdTest));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TestDto>>> Put(string id, TestDto test)
        {
            var updatedTest = await _testService.UpdateAsync(id, test);
            return Ok(new ApiResponse<TestDto>(updatedTest));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            var deletedTest = await _testService.RemoveAsync(id);
            return Ok(new ApiResponse<bool>(deletedTest));
        }
    }
}