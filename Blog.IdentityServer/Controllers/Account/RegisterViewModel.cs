using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "昵称")]
        public string RealName { get; set; }

        [Required]
        [Display(Name = "登录名")]
        public string LoginName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "The 密码 and 确认密码 do not match.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "性别")]
        public int Sex { get; set; } =0;

        [Display(Name = "生日")]
        public DateTime Birth { get; set; } = DateTime.Now;
    }
}
