using AdCore.Dto;
using AdCore.Enums;
using AdCore.Response;
using AdRepository.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdApi.Controllers
{
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GraphClient _graphClient;

        public AuthController(GraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUser()
        {
            return Ok(new ApiResponse<IList<UserDto>>(await _graphClient.ListUsers()));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(string userId)
        {
            return Ok(new ApiResponse<UserDto>(await _graphClient.GetUserById(userId)));
        }

        [HttpGet("user/{userId}/role")]
        public async Task<ActionResult<ApiResponse<string>>> GetUserRoleById(string userId)
        {
            return Ok(new ApiResponse<string>(await _graphClient.GetUserRoleById(userId)));
        }

        [HttpGet("adRole/user/{userId}/role/{roleId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateUserRole(string userId, Roles roleId)
        {
            return Ok(new ApiResponse<bool>(await _graphClient.AddUserRole(userId, roleId)));
        }

        [AllowAnonymous]
        [HttpGet("adRole/user/{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> AddUserRole(string userId)
        {
            return Ok(new ApiResponse<bool>(await _graphClient.AddUserRole(userId, Roles.User)));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody]UserModel user)
        {
            return Ok(new ApiResponse<UserDto>(await _graphClient.CreateUserWithCustomAttribute(user)));
        }
        
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UserUpdateModel user)
        {
            await _graphClient.UpdateUser(userId, user);
            return Ok(new ApiResponse<bool>(true));
        }
        
        [HttpDelete("{userId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string userId)
        {
            await _graphClient.DeleteUserById(userId);
            return Ok(new ApiResponse<bool>(true));
        }
    }
}
