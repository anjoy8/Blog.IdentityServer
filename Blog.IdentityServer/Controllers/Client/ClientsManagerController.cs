using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Common.Helper;
using Blog.IdentityServer.Models;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static IdentityModel.OidcConstants;

namespace Blog.IdentityServer.Controllers.Client
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ClientsManagerController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public ClientsManagerController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _configurationDbContext.Clients
                .Include(d => d.AllowedGrantTypes)
                .Include(d => d.AllowedScopes)
                .Include(d => d.AllowedCorsOrigins)
                .Include(d => d.RedirectUris)
                .Include(d => d.PostLogoutRedirectUris)
                .ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            //var jsontest = JsonHelper.GetJSON<IdentityServer4.Models.Client>(new IdentityServer4.Models.Client
            //{
            //    ClientId = "mvc",
            //    Description = "mvc示例模型",
            //    RequireConsent = false,
            //    ClientSecrets = new[] { new Secret("111111".Sha256()) },
            //    AllowedGrantTypes = IdentityServer4.Models.GrantTypes.Implicit,
            //    RedirectUris = { "http://admin.wmowm.com/signin-oidc" },
            //    PostLogoutRedirectUris = { "http://admin.wmowm.com/signout-callback-oidc" },
            //    AllowedScopes = new List<string>
            //        {
            //            IdentityServerConstants.StandardScopes.OpenId,
            //            IdentityServerConstants.StandardScopes.Profile
            //        },
            //    AllowAccessTokensViaBrowser = true // can return access_token to this client
            //});

            //ViewBag.DemoClient = jsontest;
            return View();
        }

        [HttpPost]
        public async Task<MessageModel<string>> Create(IdentityServer4.Models.Client request)
        {
            //var request = Newtonsoft.Json.JsonConvert.DeserializeObject<IdentityServer4.Models.Client>($"[{jsonstring}]");
            var model = (await _configurationDbContext.Clients.AddAsync(request.ToEntity())).Entity;
            await _configurationDbContext.SaveChangesAsync();

            return new MessageModel<string>()
            {
                success = true,
                msg = "添加成功",
            };
        }
    }
}