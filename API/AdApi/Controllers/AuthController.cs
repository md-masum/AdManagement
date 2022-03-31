using AdCore.Dto;
using AdCore.Enums;
using AdCore.Response;
using AdRepository.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace AdApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GraphClient _graphClient;

        public AuthController(GraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<object>>>> GetAllUser()
        {
            return Ok(new ApiResponse<List<object>>(await _graphClient.ListUsers()));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> GetUserById(string userId)
        {
            return Ok(new ApiResponse<object>(await _graphClient.GetUser(userId)));
        }

        [HttpGet("user/{userId}/role")]
        public async Task<ActionResult<ApiResponse<string>>> GetUserRoleById(string userId)
        {
            var user = (User)await _graphClient.GetUser(userId);
            return Ok(new ApiResponse<string>(_graphClient.GetUserRole(user)));
        }

        [HttpGet("user/{userId}/role/{roleId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateUserRole(string userId, Roles roleId)
        {
            return Ok(new ApiResponse<bool>(await _graphClient.AddUserRole(userId, roleId)));
        }

        [AllowAnonymous]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> AddUserRole(string userId)
        {
            return Ok(new ApiResponse<bool>(await _graphClient.AddUserRole(userId, Roles.User)));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> CreateUser([FromBody]UserModel user)
        {
            return Ok(new ApiResponse<object>(await _graphClient.CreateUserWithCustomAttribute(user)));
        }
        
        [HttpPut("{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUser(string userId, UserUpdateModel user)
        {
            return Ok(new ApiResponse<object>(await _graphClient.UpdateUser(userId, user)));
        }
        
        [HttpDelete("{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string userId)
        {
            await _graphClient.DeleteUserById(userId);
            return Ok(new ApiResponse<bool>(true));
        }
    }
}
