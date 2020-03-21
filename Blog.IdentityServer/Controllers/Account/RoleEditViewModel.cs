using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RoleEditViewModel
    {
        public RoleEditViewModel()
        {
           
        }
        public RoleEditViewModel(string Id, string Name)
        {
            this.Id = Id;
            this.RoleName = Name;
        }

        public string Id { get; set; }

        [Required]
        [Display(Name = "角色名")]
        public string RoleName { get; set; }

       
    }
}
