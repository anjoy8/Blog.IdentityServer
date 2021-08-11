// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using IdentityServer4.Events;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Extensions;
using System.Security.Principal;
using System.Security.Claims;
using IdentityModel;
using System.Linq;
using System;
using System.Collections.Generic;
using Blog.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Blog.Core.Common.Helper;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        [Route("oauth2/authorize")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return await ExternalLogin(vm.ExternalLoginScheme, returnUrl);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("oauth2/authorize")]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var user = _userManager.Users.FirstOrDefault(d => (d.LoginName == model.Username || d.Email == model.Username) && !d.tdIsDelete);

                if (user != null && !user.tdIsDelete)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.LoginName, model.Password, model.RememberLogin, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));

                        // make sure the returnUrl is still valid, and if so redirect back to authorize endpoint or a local page
                        // the IsLocalUrl check is only necessary if you want to support additional local pages, otherwise IsValidReturnUrl is more strict
                        if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        return Redirect("~/");
                    }
                    else
                    {
                        var result2 = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberLogin, lockoutOnFailure: true);

                        if (result2.Succeeded)
                        {
                            await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));

                            // make sure the returnUrl is still valid, and if so redirect back to authorize endpoint or a local page
                            // the IsLocalUrl check is only necessary if you want to support additional local pages, otherwise IsValidReturnUrl is more strict
                            if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                            {
                                return Redirect(model.ReturnUrl);
                            }

                            return Redirect("~/");
                        }
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));

                ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
            {
                // windows authentication needs special handling
                return await ProcessWindowsLoginAsync(returnUrl);
            }
            else
            {
                // start challenge and roundtrip the return URL and 
                var props = new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("ExternalLoginCallback"),
                    Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", provider },
                    }
                };
                return Challenge(props, provider);
            }
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result);
            if (user == null)
            {
                // this might be where you might initiate a custom workflow for user registration
                // in this sample we don't show how that would be done, as our sample implementation
                // simply auto-provisions new external user
                user = await AutoProvisionUserAsync(provider, providerUserId, claims);
            }

            // this allows us to collect any additonal claims or properties
            // for the specific prtotocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            // we must issue the cookie maually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            additionalLocalClaims.AddRange(principal.Claims);
            var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id.ToString();
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id.ToString(), name));

            //await HttpContext.SignInAsync(user.Id.ToString(), name, provider, localSignInProps, additionalLocalClaims.ToArray());

            var isuser = new IdentityServerUser(user.Id.ToString())
            {
                DisplayName = name,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);


            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // validate return URL and redirect back to authorization endpoint or a local page
            var returnUrl = result.Properties.Items["returnUrl"];
            if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                    ExternalProviders = new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                };
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                            (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        private async Task<IActionResult> ProcessWindowsLoginAsync(string returnUrl)
        {
            // see if windows auth has already been requested and succeeded
            var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
            if (result?.Principal is WindowsPrincipal wp)
            {
                // we will issue the external cookie and then redirect the
                // user back to the external callback, in essence, tresting windows
                // auth the same as any other external authentication mechanism
                var props = new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("ExternalLoginCallback"),
                    Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", AccountOptions.WindowsAuthenticationSchemeName },
                    }
                };

                var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
                id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                // add the groups as claims -- be careful if the number of groups is too large
                if (AccountOptions.IncludeWindowsGroups)
                {
                    var wi = wp.Identity as WindowsIdentity;
                    var groups = wi.Groups.Translate(typeof(NTAccount));
                    var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                    id.AddClaims(roles);
                }

                await HttpContext.SignInAsync(
                    IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    new ClaimsPrincipal(id),
                    props);
                return Redirect(props.RedirectUri);
            }
            else
            {
                // trigger windows auth
                // since windows auth don't support the redirect uri,
                // this URL is re-triggered when we call challenge
                return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
            }
        }

        private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)>
            FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await _userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            // user's display name
            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (name != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, name));
            }
            else
            {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // email
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Email, email));
            }

            var user = new ApplicationUser
            {
                UserName = Guid.NewGuid().ToString(),
            };
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            if (filtered.Any())
            {
                identityResult = await _userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
            }

            identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            return user;
        }

        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }
        }

        private void ProcessLoginCallbackForWsFed(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
        }

        private void ProcessLoginCallbackForSaml2p(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
        }




        [HttpGet]
        [Route("account/register")]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("account/register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null, string rName = "AdminTest")
        {
            ViewData["ReturnUrl"] = returnUrl;
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var userItem = _userManager.FindByNameAsync(model.LoginName).Result;

                if (userItem == null)
                {

                    var user = new ApplicationUser
                    {
                        Email = model.Email,
                        UserName = model.LoginName,
                        LoginName = model.RealName,
                        sex = model.Sex,
                        age = model.Birth.Year - DateTime.Now.Year,
                        birth = model.Birth,
                        FirstQuestion = model.FirstQuestion,
                        SecondQuestion = model.SecondQuestion,
                        addr = "",
                        tdIsDelete = false
                    };


                    result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        //var claims = new List<Claim>{
                        //            new Claim(JwtClaimTypes.Name, model.RealName),
                        //            new Claim(JwtClaimTypes.Email, model.Email),
                        //            new Claim(JwtClaimTypes.EmailVerified, "false", ClaimValueTypes.Boolean),
                        //            new Claim("rolename", rName),
                        //        };

                        //claims.AddRange((new List<int> { 6 }).Select(s => new Claim(JwtClaimTypes.Role, s.ToString())));


                        //result = _userManager.AddClaimsAsync(userItem, claims).Result;


                        result = await _userManager.AddClaimsAsync(user, new Claim[]{
                            new Claim(JwtClaimTypes.Name, model.RealName),
                            new Claim(JwtClaimTypes.Email, model.Email),
                            new Claim(JwtClaimTypes.EmailVerified, "false", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.Role, "6"),
                            new Claim("rolename", rName),
                        });

                        if (result.Succeeded)
                        {
                            // 可以直接登录
                            //await _signInManager.SignInAsync(user, isPersistent: false);

                            return RedirectToLocal(returnUrl);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"{userItem?.UserName} already exists");

                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpGet]
        [Route("account/users")]
        [Authorize]
        public IActionResult Users(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var users = _userManager.Users.Where(d => !d.tdIsDelete).OrderByDescending(d => d.Id).Take(50).ToList();

            return View(users);
        }



        [HttpGet("{id}")]
        [Route("account/edit/{id}")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> Edit(string id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(new EditViewModel(user.Id.ToString(), user.LoginName, user.UserName, user.Email, await _userManager.GetClaimsAsync(user), user.FirstQuestion, user.SecondQuestion));
        }


        [HttpPost]
        [Route("account/edit/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> Edit(EditViewModel model, string id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var userItem = _userManager.FindByIdAsync(model.Id).Result;

                if (userItem != null)
                {
                    var oldName = userItem.LoginName;
                    var oldEmail = userItem.Email;

                    userItem.UserName = model.LoginName;
                    userItem.LoginName = model.UserName;
                    userItem.Email = model.Email;
                    userItem.RealName = model.UserName;


                    result = await _userManager.UpdateAsync(userItem);

                    if (result.Succeeded)
                    {
                        var removeClaimsIdRst = await _userManager.RemoveClaimsAsync(userItem,
                            new Claim[]{
                                new Claim(JwtClaimTypes.Name, oldName),
                                new Claim(JwtClaimTypes.Email, oldEmail),
                        });

                        if (removeClaimsIdRst.Succeeded)
                        {
                            var addClaimsIdRst = await _userManager.AddClaimsAsync(userItem,
                                new Claim[]{
                                    new Claim(JwtClaimTypes.Name, userItem.LoginName),
                                    new Claim(JwtClaimTypes.Email, userItem.Email),
                            });

                            if (addClaimsIdRst.Succeeded)
                            {
                                return RedirectToLocal(returnUrl);
                            }
                            else
                            {
                                AddErrors(addClaimsIdRst);

                            }
                        }
                        else
                        {
                            AddErrors(removeClaimsIdRst);

                        }


                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"{userItem?.UserName} no exist!");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        [HttpGet]
        [Route("account/my")]
        [Authorize]
        public async Task<IActionResult> My(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var id = (int.Parse)(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value);
            if (id <= 0)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            return View(new EditViewModel(user.Id.ToString(), user.LoginName, user.UserName, user.Email, await _userManager.GetClaimsAsync(user), user.FirstQuestion, user.SecondQuestion));
        }

        [HttpPost]
        [Route("account/my")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> My(EditViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var id = (int.Parse)(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value);
                if (id <= 0)
                {
                    return NotFound();
                }

                // id为当前登录人
                if (model.Id == id.ToString())
                {
                    var userItem = _userManager.FindByIdAsync(model.Id).Result;
                    if (userItem != null)
                    {


                        var oldName = userItem.LoginName;
                        var oldEmail = userItem.Email;

                        userItem.UserName = model.LoginName;
                        userItem.LoginName = model.UserName;
                        userItem.Email = model.Email;
                        userItem.RealName = model.UserName;
                        userItem.FirstQuestion = model.FirstQuestion;
                        userItem.SecondQuestion = model.SecondQuestion;


                        result = await _userManager.UpdateAsync(userItem);

                        if (result.Succeeded)
                        {
                            var removeClaimsIdRst = await _userManager.RemoveClaimsAsync(userItem,
                                new Claim[]{
                                new Claim(JwtClaimTypes.Name, oldName),
                                new Claim(JwtClaimTypes.Email, oldEmail),
                            });

                            if (removeClaimsIdRst.Succeeded)
                            {
                                var addClaimsIdRst = await _userManager.AddClaimsAsync(userItem,
                                    new Claim[]{
                                    new Claim(JwtClaimTypes.Name, userItem.LoginName),
                                    new Claim(JwtClaimTypes.Email, userItem.Email),
                                });

                                if (addClaimsIdRst.Succeeded)
                                {
                                    return RedirectToLocal(returnUrl);
                                }
                                else
                                {
                                    AddErrors(addClaimsIdRst);

                                }
                            }
                            else
                            {
                                AddErrors(removeClaimsIdRst);

                            }


                        }


                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"{userItem?.UserName} no exist!");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"您无权修改该数据!");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        [Route("account/delete/{id}")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<JsonResult> Delete(string id)
        {
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var userItem = _userManager.FindByIdAsync(id).Result;

                if (userItem != null)
                {
                    userItem.tdIsDelete = true;


                    result = await _userManager.UpdateAsync(userItem);

                    if (result.Succeeded)
                    {
                        return Json(result);
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"{userItem?.UserName} no exist!");
                }

                AddErrors(result);
            }

            return Json(result.Errors);

        }

        [HttpGet]
        [Route("account/confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [Route("account/forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("account/forgot-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var roleName = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "rolename")?.Value;
                if (email == model.Email || (roleName == "SuperAdmin"))
                {

                    var user = await _userManager.FindByEmailAsync(model.Email);
                    //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    if (user == null)
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        return RedirectToAction(nameof(ForgotPasswordConfirmation), new { ResetPassword = "邮箱不存在！" });
                    }

                    // For more information on how to enable account confirmation and password reset please
                    // visit https://go.microsoft.com/fwlink/?LinkID=532713
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var accessCode = MD5Helper.MD5Encrypt32(user.Id + code);
                    var callbackUrl = Url.ResetPasswordCallbackLink(user.Id.ToString(), code, Request.Scheme, accessCode);

                    var ResetPassword = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>";

                    return RedirectToAction(nameof(ForgotPasswordConfirmation), new { ResetPassword = ResetPassword });
                }
                else if (!string.IsNullOrEmpty(model.FirstQuestion) && !string.IsNullOrEmpty(model.SecondQuestion))
                {
                    var user = _userManager.Users.FirstOrDefault(d => d.Email == model.Email && d.FirstQuestion == model.FirstQuestion && d.SecondQuestion == model.SecondQuestion);
                    if (user == null)
                    {
                        return RedirectToAction(nameof(ForgotPasswordConfirmation), new { ResetPassword = "密保答案错误！" });
                    }

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var accessCode = MD5Helper.MD5Encrypt32(user.Id + code);
                    var callbackUrl = Url.ResetPasswordCallbackLink(user.Id.ToString(), code, Request.Scheme, accessCode);


                    var ResetPassword = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>";

                    return RedirectToAction(nameof(ForgotPasswordConfirmation), new { ResetPassword = ResetPassword });
                }
                else
                {
                    var forgetPwdUrl = "https://github.com/anjoy8/Blog.IdentityServer/issues";
                    return RedirectToAction(nameof(AccessDenied), new { errorMsg = $"只能在登录状态下或者输入正确密保的情况下，修改密码! <br>如果忘记密码，请联系超级管理员手动重置：<a href='{forgetPwdUrl}'>link</a>，提Issue" });
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [Route("account/forgot-password-confirmation")]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation(string ResetPassword)
        {
            ViewBag.ResetPassword = ResetPassword;
            return View();
        }

        [HttpGet]
        [Route("account/reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null, string accessCode = null, string userId = "")
        {
            if (code == null || accessCode == null)
            {
                return RedirectToAction(nameof(AccessDenied), new { errorMsg = "code与accessCode必须都不能为空！" });
            }
            var model = new ResetPasswordViewModel { Code = code, AccessCode = accessCode, userId = userId };
            return View(model);
        }

        [HttpPost]
        [Route("account/reset-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }


            // 防止篡改
            var getAccessCode = MD5Helper.MD5Encrypt32(model.userId + model.Code);
            if (getAccessCode != model.AccessCode)
            {
                return RedirectToAction(nameof(AccessDenied), new { errorMsg = "随机码已被篡改！密码重置失败！" });
            }

            if (user != null && user.Id.ToString() != model.userId)
            {
                return RedirectToAction(nameof(AccessDenied), new { errorMsg = "不能修改他人邮箱！密码重置失败！" });
            }


            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [Route("account/reset-password-confirmation")]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        //[Route("account/access-denied")]
        public IActionResult AccessDenied(string errorMsg = "")
        {
            ViewBag.ErrorMsg = errorMsg;
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


        // Role Manager



        [HttpGet]
        [Route("account/Roleregister")]
        public IActionResult RoleRegister(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("account/Roleregister")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleRegister(RoleRegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var roleItem = _roleManager.FindByNameAsync(model.RoleName).Result;

                if (roleItem == null)
                {

                    var role = new ApplicationRole
                    {
                        Name = model.RoleName
                    };


                    result = await _roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {

                        if (result.Succeeded)
                        {
                            // 可以直接登录
                            //await _signInManager.SignInAsync(user, isPersistent: false);

                            return RedirectToLocal(returnUrl);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"{roleItem?.Name} already exists");

                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpGet]
        [Route("account/Roles")]
        [Authorize]
        public IActionResult Roles(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var roles = _roleManager.Roles.Where(d => !d.IsDeleted).ToList();

            return View(roles);
        }



        [HttpGet("{id}")]
        [Route("account/Roleedit/{id}")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> RoleEdit(string id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (id == null)
            {
                return NotFound();
            }

            var user = await _roleManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(new RoleEditViewModel(user.Id.ToString(), user.Name));
        }


        [HttpPost]
        [Route("account/Roleedit/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> RoleEdit(RoleEditViewModel model, string id, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var roleItem = _roleManager.FindByIdAsync(model.Id).Result;

                if (roleItem != null)
                {
                    roleItem.Name = model.RoleName;


                    result = await _roleManager.UpdateAsync(roleItem);

                    if (result.Succeeded)
                    {
                        return RedirectToLocal(returnUrl);
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"{roleItem?.Name} no exist!");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        [HttpPost]
        [Route("account/Roledelete/{id}")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<JsonResult> RoleDelete(string id)
        {
            IdentityResult result = new IdentityResult();

            if (ModelState.IsValid)
            {
                var roleItem = _roleManager.FindByIdAsync(id).Result;

                if (roleItem != null)
                {
                    roleItem.IsDeleted = true;


                    result = await _roleManager.UpdateAsync(roleItem);

                    if (result.Succeeded)
                    {
                        return Json(result);
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"{roleItem?.Name} no exist!");
                }

                AddErrors(result);
            }

            return Json(result.Errors);

        }
    }
}
