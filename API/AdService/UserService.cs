using AdCore.Dto.Users;
using AdCore.Enums;
using AdCore.Interface;
using AdRepository.Authentication;
using AdRepository.Interface;
using AdService.Base;
using AdService.Interface;
using AutoMapper;
using Microsoft.Extensions.Logging;
using User = AdCore.Entity.User;

namespace AdService
{
    public class UserService : BaseService<User, UserDto>, IUserService
    {
        private readonly GraphClient _graphClient;
        private readonly ILogger<UserService> _logger;
        private readonly ICurrentUserInfoService _currentUserService;

        public UserService(GraphClient graphClient,
            ICosmosDbRepository<User> baseRepository,
            IMapper mapper,
            ILogger<UserService> logger,
            ICurrentUserInfoService currentUserService) : base(baseRepository, mapper)
        {
            _currentUserService = currentUserService;
            _graphClient = graphClient;
            _logger = logger;
        }
        public async Task<UserDto> GetCurrentUser()
        {
            return await _currentUserService.CurrentLoggedInUser();
        }

        public async Task<UserDto> UpdateUser(UserUpdateModel user)
        {
            var userRole = await _graphClient.GetUserRoleById(_currentUserService.UserId);
            user.Role = (Roles)Enum.Parse(typeof(Roles), userRole);
            await _graphClient.UpdateUser(_currentUserService.UserId, user);
            return await _currentUserService.CurrentLoggedInUser();
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

            var userDetails = await GetByIdAsync(adB2CId);
            if (userDetails is not null)
            {
                _logger.LogError($"User is already present");
                return;
            }

            var userToCreate = new User
            {
                AdB2CId = adB2CId,
                Id = adB2CId
            };

            await BaseRepository.AddAsync(userToCreate);
        }
    }
}
