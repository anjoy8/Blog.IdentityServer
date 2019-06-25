using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Blog.IdentityServer;
using Blog.IdentityServer.Data;
using Blog.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Blog.Core.Common.Helper;
using System.Collections.Generic;

namespace Blog.IdentityServer
{
    public class SeedData
    {
        private static string GitJsonFileFormat = "https://github.com/anjoy8/Blog.Data.Share/raw/master/Blog.Core.Data.json/{0}.tsv";

        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            //1.dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
            //2.dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
            //3.dotnet ef migrations add AppDbMigration -c ApplicationDbContext -o Data
            //4.dotnet run /seed
            Console.WriteLine("Seeding database...");

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                {
                    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    context.Database.Migrate();
                    EnsureSeedData(context);
                }

                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    var BlogCore_Users = JsonHelper.ParseFormByJson<List<sysUserInfo>>(GetNetData.Get(string.Format(GitJsonFileFormat, "sysUserInfo")));
                    var BlogCore_Roles = JsonHelper.ParseFormByJson<List<Role>>(GetNetData.Get(string.Format(GitJsonFileFormat, "Role")));
                    var BlogCore_UserRoles = JsonHelper.ParseFormByJson<List<UserRole>>(GetNetData.Get(string.Format(GitJsonFileFormat, "UserRole")));

                    foreach (var item in BlogCore_Users)
                    {
                        if (item == null || item.uLoginName == null)
                        {
                            continue;
                        }
                        var userItem = userMgr.FindByNameAsync(item.uLoginName).Result;
                        var rid = BlogCore_UserRoles.FirstOrDefault(d => d.UserId == item.uID)?.RoleId;
                        var rName = BlogCore_Roles.FirstOrDefault(d => d.Id == rid)?.Name;

                        if (userItem == null)
                        {
                            if (rid > 0 && !string.IsNullOrEmpty(rName))
                            {
                                userItem = new ApplicationUser
                                {
                                    UserName = item.uLoginName,
                                    LoginName = item.uRealName,
                                    sex = item.sex,
                                    age = item.age,
                                    birth = item.birth,
                                    addr = item.addr,
                                    tdIsDelete = item.tdIsDelete

                                };

                                //var result = userMgr.CreateAsync(userItem, "BlogIdp123$" + item.uLoginPWD).Result;

                                // 因为导入的密码是 MD5密文，所以这里统一都用初始密码了
                                var result = userMgr.CreateAsync(userItem, "BlogIdp123$InitPwd").Result;
                                if (!result.Succeeded)
                                {
                                    throw new Exception(result.Errors.First().Description);
                                }

                                result = userMgr.AddClaimsAsync(userItem, new Claim[]{
                            new Claim(JwtClaimTypes.Name, item.uRealName),
                            new Claim(JwtClaimTypes.Email, $"{item.uLoginName}@email.com"),
                            new Claim(JwtClaimTypes.Role, rName)
                        }).Result;

                                if (!result.Succeeded)
                                {
                                    throw new Exception(result.Errors.First().Description);
                                }
                                Console.WriteLine($"{userItem?.UserName} created");//AspNetUserClaims 表

                            }
                            else
                            {
                                Console.WriteLine($"{item?.uLoginName} doesn't have a corresponding role.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{userItem?.UserName} already exists");
                        }

                    }

                }
            }

            Console.WriteLine("Done seeding database.");
            Console.WriteLine();
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Console.WriteLine("Clients being populated");
                foreach (var client in Config.GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Console.WriteLine("IdentityResources being populated");
                foreach (var resource in Config.GetIdentityResources().ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("IdentityResources already populated");
            }

            if (!context.ApiResources.Any())
            {
                Console.WriteLine("ApiResources being populated");
                foreach (var resource in Config.GetApiResources().ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("ApiResources already populated");
            }
        }
    }
}
