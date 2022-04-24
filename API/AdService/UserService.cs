using AdCore.Dto.Users;
using AdCore.Entity;
using AdCore.Enums;
using AdCore.Interface;
using AdRepository.Authentication;
using AdRepository.Interface;
using AdService.Base;
using AdService.Interface;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AdService
{
    public class UserService : BaseService<User, UserDto>, IUserService
    {
        private readonly GraphClient _graphClient;
        private readonly ILogger<UserService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UserService(GraphClient graphClient,
            ICosmosDbRepository<User> baseRepository,
            IMapper mapper,
            ILogger<UserService> logger,
            ICurrentUserService currentUserService) : base(baseRepository, mapper)
        {
            _graphClient = graphClient;
            _logger = logger;
            _currentUserService = currentUserService;
        }
        public async Task<UserDto> GetCurrentUser()
        {
            return await GetCurrentUserDetails();
        }

        public async Task<UserDto> UpdateUser(UserUpdateModel user)
        {
            var userRole = await _graphClient.GetUserRoleById(_currentUserService.UserId);
            user.Role = (Roles)Enum.Parse(typeof(Roles), userRole);
            await _graphClient.UpdateUser(_currentUserService.UserId, user);
            return await GetCurrentUserDetails();
        }

        public async Task<bool> ChangePassword(string password)
        {
            await _graphClient.SetPasswordByUserId(_currentUserService.UserId, password);
            return true;
        }

        public async Task AddUser(string adB2CId)
        {
            var user = await _graphClient.GetUserById(_currentUserService.UserId);
            if(user is null)
            {
                _logger.LogError($"No user found by id {adB2CId}");
                return;
            }

            var userDetails = await GetAsync(c => c.AdB2CId == _currentUserService.UserId);
            if (userDetails is not null)
            {
                _logger.LogError($"User is already present");
                return;
            }

            var userToCreate = new User
            {
                AdB2CId = adB2CId
            };

            await BaseRepository.AddAsync(userToCreate);
        }

        private async Task<UserDto> GetCurrentUserDetails()
        {
            var user = await GetAsync(c => c.AdB2CId == _currentUserService.UserId);
            user.Role = _currentUserService.Roles;
            user.FirstName = _currentUserService.FirstName;
            user.LastName = _currentUserService.LastName;
            user.DisplayName = _currentUserService.UserName;
            user.Email = _currentUserService.Email;
            return user;
        }
    }
}
