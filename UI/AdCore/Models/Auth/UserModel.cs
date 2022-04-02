using System.ComponentModel.DataAnnotations;
using AdCore.Enums;
using AdCore.MapperProfile;

namespace AdCore.Models.Auth
{
    public class UserModel : IMapFrom<UserDto>
    {
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public Roles Role { get; set; } = Roles.User;

        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
