using AdCore.Dto.Users;
using AdCore.Response;
using AdService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
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
        public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromForm] UserUpdateModel user)
        {
            return Ok(new ApiResponse<UserDto>(await _userService.UpdateUser(user)));
        }

        [HttpPost("changeUserPassword")]
        public async Task<ActionResult<bool>> ChangeUserPassword([FromBody]string password)
        {
            return Ok(new ApiResponse<bool>(await _userService.ChangePassword(password)));
        }

        [AllowAnonymous]
        [HttpPost("addUser")]
        public async Task<ActionResult<bool>> AddUser([FromBody] string adB2CId)
        {
            await _userService.AddUser(adB2CId);
            return Ok();
        }
    }
}
