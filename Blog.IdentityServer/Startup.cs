using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Blog.IdentityServer.Data;
using Blog.IdentityServer.Models;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Blog.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Blog.IdentityServer.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using IdentityServer4.Extensions;

namespace Blog.IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSameSiteCookiePolicy();

            string connectionStringFile = Configuration.GetConnectionString("DefaultConnection_file");
            var connectionString = File.Exists(connectionStringFile) ? File.ReadAllText(connectionStringFile).Trim() : Configuration.GetConnectionString("DefaultConnection");
            var isMysql = Configuration.GetConnectionString("IsMysql").ObjToBool();


            if (connectionString == "")
            {
                throw new Exception("数据库配置异常");
            }
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            if (isMysql)
            {
                // mysql
                services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString));
            }
            else
            {
                // sqlserver
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            };


            services.Configure<IdentityOptions>(
              options =>
              {
                  //options.Password.RequireDigit = false;
                  //options.Password.RequireLowercase = false;
                  //options.Password.RequireNonAlphanumeric = false;
                  //options.Password.RequireUppercase = false;
                  //options.SignIn.RequireConfirmedEmail = false;
                  //options.SignIn.RequireConfirmedPhoneNumber = false;
                  //options.User.AllowedUserNameCharacters = null;
              });

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
               {
                   options.User = new UserOptions
                   {
                       RequireUniqueEmail = true, //要求Email唯一
                       AllowedUserNameCharacters = null //允许的用户名字符
                   };
                   options.Password = new PasswordOptions
                   {
                       RequiredLength = 8, //要求密码最小长度，默认是 6 个字符
                       RequireDigit = true, //要求有数字
                       RequiredUniqueChars = 3, //要求至少要出现的字母数
                       RequireLowercase = true, //要求小写字母
                       RequireNonAlphanumeric = false, //要求特殊字符
                       RequireUppercase = false //要求大写字母
                   };

                   //options.Lockout = new LockoutOptions
                   //{
                   //    AllowedForNewUsers = true, // 新用户锁定账户
                   //    DefaultLockoutTimeSpan = TimeSpan.FromHours(1), //锁定时长，默认是 5 分钟
                   //    MaxFailedAccessAttempts = 3 //登录错误最大尝试次数，默认 5 次
                   //};
                   //options.SignIn = new SignInOptions
                   //{
                   //    RequireConfirmedEmail = true, //要求激活邮箱
                   //    RequireConfirmedPhoneNumber = true //要求激活手机号
                   //};
                   //options.ClaimsIdentity = new ClaimsIdentityOptions
                   //{
                   //    // 这里都是修改相应的Cliams声明的
                   //    RoleClaimType = "IdentityRole",
                   //    UserIdClaimType = "IdentityId",
                   //    SecurityStampClaimType = "SecurityStamp",
                   //    UserNameClaimType = "IdentityName"
                   //};
               })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/oauth2/authorize");
            });


            //配置session的有效时间,单位秒
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30);
            });

            services.AddMvc();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    options.ForwardedHeaders =
            //        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            //    options.KnownNetworks.Clear();
            //    options.KnownProxies.Clear();
            //});

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    // 查看发现文档
                    if (Configuration["StartUp:IsOnline"].ObjToBool())
                    {
                        options.IssuerUri = Configuration["StartUp:OnlinePath"].ObjToString();
                    }
                    options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                    {
                        LoginUrl = "/oauth2/authorize",//登录地址  
                    };
                })

                // 自定义验证，可以不走Identity
                //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddExtensionGrantValidator<WeiXinOpenGrantValidator>()

                // 数据库模式
                .AddAspNetIdentity<ApplicationUser>()

                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    if (isMysql)
                    {
                        options.ConfigureDbContext = b => b.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    }
                    else
                    {
                        options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    }
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    if (isMysql)
                    {
                        options.ConfigureDbContext = b => b.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    }
                    else
                    {
                        options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    }

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // frequency in seconds to cleanup stale grants. 15 is useful during debugging
                });

            builder.AddDeveloperSigningCredential();
            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }


            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.Requirements.Add(new ClaimRequirement("rolename", "Admin")));
                options.AddPolicy("SuperAdmin", policy => policy.Requirements.Add(new ClaimRequirement("rolename", "SuperAdmin")));
            });

            services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();

            services.AddIpPolicyRateLimitSetup(Configuration);


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (ctx, next) =>
            {
                if (Configuration["StartUp:IsOnline"].ObjToBool())
                {
                    ctx.SetIdentityServerOrigin(Configuration["StartUp:OnlinePath"].ObjToString());
                }
                await next();
            });

            app.UseIpLimitMildd();
         
            //app.UseForwardedHeaders();
            app.UseCookiePolicy();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
           {
               endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
           });
        }
    }
}
