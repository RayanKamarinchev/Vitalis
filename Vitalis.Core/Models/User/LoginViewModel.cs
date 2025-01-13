using System.ComponentModel.DataAnnotations;

namespace Vitalis.Core.Models.User
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public bool RememberMe { get; set; }
    }
}
