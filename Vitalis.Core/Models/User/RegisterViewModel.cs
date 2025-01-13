using System.ComponentModel.DataAnnotations;
using Vitalis.Core.Infrastructure;

namespace Vitalis.Core.Models.User
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(Constants.EmailMaxLength, MinimumLength = Constants.EmailMinLength)]
        public string Email { get; set; }
        [Required]
        [StringLength(Constants.PasswordMaxLength, MinimumLength = Constants.PasswordMinLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        [StringLength(Constants.NameMaxLength, MinimumLength = Constants.NameMinLength)]
        public string Name { get; set; }
    }
}
