using AdCore.Entity;
using AdCore.Enums;
using AdCore.MapperProfile;

namespace AdCore.Dto.Users
{
    public class UserDto : BaseDto, IMapFrom<User>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Roles Role { get; set; }

        public string AdB2CId { get; set; }
        public string CompanyId { get; set; }

        public string Country { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        public string Thana { get; set; }
        public string Area { get; set; }
    }
}
