using AdCore.Dto;

namespace AdService.Interface
{
    public interface IUserService
    {
        Task<UserDto> GetCurrentUser();
        Task<UserDto> UpdateUser(UserUpdateModel user);
        Task<bool> ChangePassword(string password);
    }
}
