using AdCore.Dto;
using AdCore.Response;
using AdService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AdApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            return Ok(new ApiResponse<UserDto>(await _userService.GetCurrentUser()));
        }

        [HttpPost("updateCurrentUser")]
        public async Task<ActionResult<UserDto>> UpdateCurrentUser(UserUpdateModel user)
        {
            return Ok(new ApiResponse<UserDto>(await _userService.UpdateUser(user)));
        }

        [HttpGet("changeUserPassword")]
        public async Task<ActionResult<bool>> ChangeUserPassword(string password)
        {
            return Ok(new ApiResponse<bool>(await _userService.ChangePassword(password)));
        }
    }
}
