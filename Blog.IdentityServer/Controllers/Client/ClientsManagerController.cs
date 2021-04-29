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
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// 数据列表页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
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

        /// <summary>
        /// 数据修改页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "SuperAdmin")]
        public IActionResult CreateOrEdit(int id)
        {
            ViewBag.ClientId = id;
            return View();
        }


        [HttpGet]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<MessageModel<ClientDto>> GetDataById(int id = 0)
        {
            var model = (await _configurationDbContext.Clients
                .Include(d => d.AllowedGrantTypes)
                .Include(d => d.AllowedScopes)
                .Include(d => d.AllowedCorsOrigins)
                .Include(d => d.RedirectUris)
                .Include(d => d.PostLogoutRedirectUris)
                .Include(d => d.ClientSecrets)
                .ToListAsync()).FirstOrDefault(d => d.Id == id).ToModel();

            var clientDto = new ClientDto();
            if (model != null)
            {
                clientDto = new ClientDto()
                {
                    ClientId = model?.ClientId,
                    ClientName = model?.ClientName,
                    Description = model?.Description,
                    AllowAccessTokensViaBrowser = (model?.AllowAccessTokensViaBrowser).ObjToString(),
                    AllowedCorsOrigins = string.Join(",", model?.AllowedCorsOrigins),
                    AllowedGrantTypes = string.Join(",", model?.AllowedGrantTypes),
                    AllowedScopes = string.Join(",", model?.AllowedScopes),
                    PostLogoutRedirectUris = string.Join(",", model?.PostLogoutRedirectUris),
                    RedirectUris = string.Join(",", model?.RedirectUris),
                    ClientSecrets = string.Join(",", model?.ClientSecrets.Select(d => d.Value)),
                };
            }

            return new MessageModel<ClientDto>()
            {
                success = true,
                msg = "获取成功",
                response = clientDto
            };
        }

        [HttpPost]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<MessageModel<string>> SaveData(ClientDto request)
        {
            if (request != null && request.id == 0)
            {
                IdentityServer4.Models.Client client = new IdentityServer4.Models.Client()
                {
                    ClientId = request?.ClientId,
                    ClientName = request?.ClientName,
                    Description = request?.Description,
                    AllowAccessTokensViaBrowser = (request?.AllowAccessTokensViaBrowser).ObjToBool(),
                    AllowedCorsOrigins = request?.AllowedCorsOrigins?.Split(","),
                    AllowedGrantTypes = request?.AllowedGrantTypes?.Split(","),
                    AllowedScopes = request?.AllowedScopes?.Split(","),
                    PostLogoutRedirectUris = request?.PostLogoutRedirectUris?.Split(","),
                    RedirectUris = request?.RedirectUris?.Split(","),
                };

                if (!string.IsNullOrEmpty(request.ClientSecrets))
                {
                    client.ClientSecrets = new List<Secret>() { new Secret(request.ClientSecrets.Sha256()) };
                }

                var result = (await _configurationDbContext.Clients.AddAsync(client.ToEntity()));
                await _configurationDbContext.SaveChangesAsync();
            }

            if (request != null && request.id > 0)
            {

                var modelEF = (await _configurationDbContext.Clients
                    .Include(d => d.AllowedGrantTypes)
                    .Include(d => d.AllowedScopes)
                    .Include(d => d.AllowedCorsOrigins)
                    .Include(d => d.RedirectUris)
                    .Include(d => d.PostLogoutRedirectUris)
                    .ToListAsync()).FirstOrDefault(d => d.Id == request.id);

                modelEF.ClientId = request?.ClientId;
                modelEF.ClientName = request?.ClientName;
                modelEF.Description = request?.Description;
                modelEF.AllowAccessTokensViaBrowser = (request?.AllowAccessTokensViaBrowser).ObjToBool();

                var AllowedCorsOrigins = new List<IdentityServer4.EntityFramework.Entities.ClientCorsOrigin>();
                if (!string.IsNullOrEmpty(request?.AllowedCorsOrigins))
                {
                    request?.AllowedCorsOrigins.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                               {
                                   AllowedCorsOrigins.Add(new IdentityServer4.EntityFramework.Entities.ClientCorsOrigin()
                                   {
                                       Client = modelEF,
                                       ClientId = modelEF.Id,
                                       Origin = s
                                   });
                               });
                    modelEF.AllowedCorsOrigins = AllowedCorsOrigins;
                }



                var AllowedGrantTypes = new List<IdentityServer4.EntityFramework.Entities.ClientGrantType>();
                if (!string.IsNullOrEmpty(request?.AllowedGrantTypes))
                {
                    request?.AllowedGrantTypes.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                    {
                        AllowedGrantTypes.Add(new IdentityServer4.EntityFramework.Entities.ClientGrantType()
                        {
                            Client = modelEF,
                            ClientId = modelEF.Id,
                            GrantType = s
                        });
                    });
                    modelEF.AllowedGrantTypes = AllowedGrantTypes;
                }



                var AllowedScopes = new List<IdentityServer4.EntityFramework.Entities.ClientScope>();
                if (!string.IsNullOrEmpty(request?.AllowedScopes))
                {
                    request?.AllowedScopes.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                    {
                        AllowedScopes.Add(new IdentityServer4.EntityFramework.Entities.ClientScope()
                        {
                            Client = modelEF,
                            ClientId = modelEF.Id,
                            Scope = s
                        });
                    });
                    modelEF.AllowedScopes = AllowedScopes;
                }


                var PostLogoutRedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri>();
                if (!string.IsNullOrEmpty(request?.PostLogoutRedirectUris))
                {
                    request?.PostLogoutRedirectUris.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                    {
                        PostLogoutRedirectUris.Add(new IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri()
                        {
                            Client = modelEF,
                            ClientId = modelEF.Id,
                            PostLogoutRedirectUri = s
                        });
                    });
                    modelEF.PostLogoutRedirectUris = PostLogoutRedirectUris;
                }


                var RedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientRedirectUri>();
                if (!string.IsNullOrEmpty(request?.RedirectUris))
                {
                    request?.RedirectUris.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                    {
                        RedirectUris.Add(new IdentityServer4.EntityFramework.Entities.ClientRedirectUri()
                        {
                            Client = modelEF,
                            ClientId = modelEF.Id,
                            RedirectUri = s
                        });
                    });
                    modelEF.RedirectUris = RedirectUris;
                }

                var result = (_configurationDbContext.Clients.Update(modelEF));
                await _configurationDbContext.SaveChangesAsync();
            }


            return new MessageModel<string>()
            {
                success = true,
                msg = "添加成功",
            };
        }
    }
}