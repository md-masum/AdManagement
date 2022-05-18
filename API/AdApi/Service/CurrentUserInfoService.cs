using AdCore.Dto.Users;
using AdCore.Entity;
using AdCore.Enums;
using AdCore.Interface;
using AdRepository.Interface;
using AutoMapper;
using Microsoft.Azure.Cosmos;
using System.Security.Claims;
using User = AdCore.Entity.User;

namespace AdApi.Service
{
    public class CurrentUserInfoService : ICurrentUserInfoService
    {
        private readonly ICosmosDbRepository<Company> _companyRepository;
        private readonly ICosmosDbRepository<User> _useRepository;
        private readonly IMapper _mapper;

        public CurrentUserInfoService(IHttpContextAccessor httpContextAccessor,
            ICosmosDbRepository<Company> companyRepository,
            ICosmosDbRepository<User> useRepository,
            IMapper mapper)
        {
            _companyRepository = companyRepository;
            _useRepository = useRepository;
            _mapper = mapper;

            UserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserName = httpContextAccessor.HttpContext?.User.FindFirstValue("name");
            Email = httpContextAccessor.HttpContext?.User.FindFirstValue("emails");
            FirstName = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.GivenName);
            LastName = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Surname);
            var role = httpContextAccessor.HttpContext?.User.FindFirstValue("extension_Role");
            if (role != null && Enum.IsDefined(typeof(Roles), role)) Roles = Enum.Parse<Roles>(role);
        }

        public string UserId { get; }
        public string UserName { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public Roles Roles { get; }

        public async Task<bool> HasAssociateCompany()
        {
            var user = await GetCurrentUser();
            return user is not null && user.CompanyId is not null;
        }

        public async Task<bool> IsMasterSeller()
        {
            QueryDefinition query = new QueryDefinition(
                    "select * from c where c.type = @type AND c.masterSeller = @masterSeller ")
                .WithParameter("@type", nameof(Company))
                .WithParameter("@masterSeller", UserId);

            var data = await _companyRepository.GetAsync(query);
            return data is not null;
        }

        public async Task<UserDto> CurrentLoggedInUser()
        {
            var user = await GetCurrentUser();
            user.Role = Roles;
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.DisplayName = UserName;
            user.Email = Email;

            return user;
        }

        private async Task<UserDto> GetCurrentUser()
        {
            QueryDefinition query = new QueryDefinition(
                    "select * from c where c.type = @type AND c.adB2CId = @adB2CId ")
                .WithParameter("@type", nameof(User))
                .WithParameter("@adB2CId", UserId);

            var user = _mapper.Map<UserDto>(await _useRepository.GetAsync(query));
            return user;
        }
    }
}
