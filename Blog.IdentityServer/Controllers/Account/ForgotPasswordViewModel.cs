using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "密保问题一：你喜欢的动漫？")]
        public string FirstQuestion { get; set; }

        [Display(Name = "密保问题二：你喜欢的名著？")]
        public string SecondQuestion { get; set; }
    }
}
