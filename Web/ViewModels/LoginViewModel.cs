using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
        public string UserName { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
