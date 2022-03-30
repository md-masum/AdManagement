using System.Security.Claims;
using AdCore.Interface;

namespace AdApi.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserName = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            Email = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
            FirstName = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.MobilePhone);
            LastName = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.MobilePhone);
        }

        public string UserId { get; }
        public string UserName { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}
