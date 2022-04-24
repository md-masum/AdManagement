using AdCore.Dto.Users;
using AdCore.Entity;

namespace AdService.Interface
{
    public interface IUserService : IBaseService<User, UserDto>
    {
        Task<UserDto> GetCurrentUser();
        Task<UserDto> UpdateUser(UserUpdateModel user);
        Task<bool> ChangePassword(string password);

        Task AddUser(string adB2CId);
    }
}
