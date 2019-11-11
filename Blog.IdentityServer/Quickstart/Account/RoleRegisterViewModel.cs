using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RoleRegisterViewModel
    {

        [Required]
        [Display(Name = "角色名")]
        public string RoleName { get; set; }

        
    }
}
