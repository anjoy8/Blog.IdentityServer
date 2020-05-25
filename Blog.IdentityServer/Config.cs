// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Blog.IdentityServer.Extensions;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;

namespace Blog.IdentityServer
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles", "角色", new List<string> { JwtClaimTypes.Role }),
                new IdentityResource("rolename", "角色名", new List<string> { "rolename" }),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            // blog.core 项目
            return new List<ApiResource> {
                new ApiResource("blog.core.api", "Blog.Core API") {
                    // include the following using claims in access token (in addition to subject id)
                    //requires using using IdentityModel;
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Role },
                    ApiSecrets = new List<Secret>()
                    {
                        new Secret("api_secret".Sha256())
                    },
                }
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client
            return new List<Client> {
                // blog.vue 前端vue项目
                new Client {
                    ClientId = "blogvuejs",
                    ClientName = "Blog.Vue JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =           {
                        "http://vueblog.neters.club/callback",
                        "http://apk.neters.club/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = { "http://vueblog.neters.club" },
                    AllowedCorsOrigins =     { "http://vueblog.neters.club" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "blog.core.api"
                    }
                },
                // blog.admin 前端vue项目
                new Client {
                    ClientId = "blogadminjs",
                    ClientName = "Blog.Admin JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        "http://vueadmin.neters.club/callback",
                        "http://apk.neters.club/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = { "http://vueadmin.neters.club" },
                    AllowedCorsOrigins =     { "http://vueadmin.neters.club" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "blog.core.api"
                    }
                },
                // nuxt.tbug 前端nuxt项目
                new Client {
                    ClientId = "tibugnuxtjs",
                    ClientName = "Nuxt.tBug JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =           { "http://tibug.neters.club/callback" },
                    PostLogoutRedirectUris = { "http://tibug.neters.club" },
                    AllowedCorsOrigins =     { "http://tibug.neters.club" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "blog.core.api"
                    }
                },
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "chrisdddmvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,
                    AlwaysIncludeUserClaimsInIdToken=true,//将用户所有的claims包含在IdToken内
                
                    // where to redirect to after login
                    RedirectUris = { "http://ddd.neters.club/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://ddd.neters.club/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "rolename",
                    }
                },
                // 控制台客户端
                new Client
                {
                    ClientId = "Console",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = new List<string>()
                    {
                        GrantTypes.ResourceOwnerPassword.FirstOrDefault(),
                        GrantTypes.ClientCredentials.FirstOrDefault(),
                        GrantTypeCustom.ResourceWeixinOpen,
                    },

                    AllowedScopes = new List<string>
                    {
                        "blog.core.api"
                    }
                }
            };
        }
    }
}