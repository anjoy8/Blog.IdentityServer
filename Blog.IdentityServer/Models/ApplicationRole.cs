using Microsoft.AspNetCore.Identity;
using System;

namespace Blog.IdentityServer.Models
{
    // Add profile data for application roles by adding properties to the ApplicationRole class
    public class ApplicationRole : IdentityRole
    {

        public bool IsDeleted { get; set; }

    }
}
