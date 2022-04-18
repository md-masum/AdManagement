using System.ComponentModel.DataAnnotations;
using AdCore.MapperProfile;

namespace AdCore.Models.UserProfile
{
    public class ProfileUpdateModel : IMapFrom<ProfileModel>
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
