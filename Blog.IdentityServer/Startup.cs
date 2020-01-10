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
using Microsoft.IdentityModel.Tokens;
using System.IO;
using IdentityServer4.Quickstart.UI;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;

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
            if (connectionString == "")
            {
                throw new Exception("数据库配置异常");
            }
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/oauth2/authorize");
            });

            services.AddMvc();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.IssuerUri = "http://ids.neters.club";
                    options.PublicOrigin = "http://ids.neters.club";
                    options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                    {
                        LoginUrl = "/oauth2/authorize",//登录地址  
                    };
                })

                //.AddDeveloperSigningCredential(true, ConstanceHelper.AppSettings.CredentialFileName)
                //.AddSigningCredential(new X509Certificate2(Path.Combine(Environment.WebRootPath,
                //        Configuration["Certificates:Path"]),
                //        Configuration["Certificates:Password"]))

                // in-Sqlite
                .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // frequency in seconds to cleanup stale grants. 15 is useful during debugging
                });

            builder.AddDeveloperSigningCredential();
            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }


            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
