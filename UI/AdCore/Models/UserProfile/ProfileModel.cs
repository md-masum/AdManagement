using AdCore.Enums;

namespace AdCore.Models.UserProfile
{
    public class ProfileModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Roles Role { get; set; }
    }
}
