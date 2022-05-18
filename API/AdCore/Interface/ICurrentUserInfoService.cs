using AdCore.Dto.Users;
using AdCore.Enums;

namespace AdCore.Interface
{
    public interface ICurrentUserInfoService
    {
        string UserId { get; }
        string UserName { get; }
        string Email { get; }
        string FirstName { get; }
        string LastName { get; }
        Roles Roles { get; }

        Task<bool> HasAssociateCompany();
        Task<bool> IsMasterSeller();
        Task<UserDto> CurrentLoggedInUser();
    }
}
