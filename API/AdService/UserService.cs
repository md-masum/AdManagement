using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdCore.Dto;
using AdCore.Enums;
using AdCore.Exceptions;
using AdCore.Interface;
using AdRepository.Authentication;
using AdService.Interface;
using Microsoft.Extensions.Logging;

namespace AdService
{
    public class UserService : IUserService
    {
        private readonly GraphClient _graphClient;
        private readonly ICurrentUserService _currentUserService;

        public UserService(GraphClient graphClient,
            ICurrentUserService currentUserService)
        {
            _graphClient = graphClient;
            _currentUserService = currentUserService;
        }
        public async Task<UserDto> GetCurrentUser()
        {
            var user = await _graphClient.GetUserById(_currentUserService.UserId);
            return user;
        }

        public async Task<UserDto> UpdateUser(UserUpdateModel user)
        {
            var userRole = await _graphClient.GetUserRoleById(_currentUserService.UserId);
            user.Role = (Roles)Enum.Parse(typeof(Roles), userRole);
            await _graphClient.UpdateUser(_currentUserService.UserId, user);
            return await _graphClient.GetUserById(_currentUserService.UserId);
        }

        public async Task<bool> ChangePassword(string password)
        {
            await _graphClient.SetPasswordByUserId(_currentUserService.UserId, password);
            return true;
        }
    }
}
