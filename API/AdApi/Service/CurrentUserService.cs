using System.Security.Claims;
using AdCore.Enums;
using AdCore.Interface;


namespace AdApi.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
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
    }
}
