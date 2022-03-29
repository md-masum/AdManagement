using System.ComponentModel.DataAnnotations;
using AdCore.Entity;
using AdCore.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace AdApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        static readonly string[] scopeRequiredByApi = new string[] { "BlazorHostedB2CServer.Access" };
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public ActionResult<IList<Test>> Get()
        {
            return Ok(_testService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Test> GetById(string id)
        {
            return Ok(_testService.GetById(id));
        }
        [Authorize]
        [RequiredScope(RequiredScopesConfigurationKey = "BlazorHostedB2CServer.Access")]
        [HttpPost]
        public ActionResult<Test> Create(Test testToCreate)
        {
            return Ok(_testService.Create(testToCreate));
        }

        [HttpPut]
        public ActionResult<Test> Update(Test testToUpdate)
        {
            return Ok(_testService.Update(testToUpdate));
        }

        [HttpDelete("{id}")]
        public ActionResult<bool> Delete(string id)
        {
            return Ok(_testService.Delete(id));
        }
    }
}
