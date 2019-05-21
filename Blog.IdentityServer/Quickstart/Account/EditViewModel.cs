using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class EditViewModel
    {
        public EditViewModel()
        {
           
        }
        public EditViewModel(string Id, string Name, string LoginName, string Email)
        {
            this.Id = Id;
            this.LoginName = LoginName;
            this.Email = Email;
            this.UserName = Name;
        }

        public string Id { get; set; }

        [Required]
        [Display(Name = "昵称")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "登录名")]
        public string LoginName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }
        



        [Display(Name = "性别")]
        public int Sex { get; set; } =0;

        [Display(Name = "生日")]
        public DateTime Birth { get; set; } = DateTime.Now;
    }
}
