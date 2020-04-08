using Blog.IdentityServer.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.IdentityServer.Extensions
{
    public class WeiXinOpenGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => GrantTypeCustom.ResourceWeixinOpen;

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            try
            {
                // 参数获取
                var openId = context.Request.Raw["openid"];
                var unionId = context.Request.Raw["unionid"];
                var userName = context.Request.Raw["user_name"];

                // 通过openId和unionId 参数来进行数据库的相关验证
                var claimList = await ValidateUserAsync(openId, unionId);

                //授权通过返回
                context.Result = new GrantValidationResult
                (
                    subject: openId,
                    authenticationMethod: "custom",
                    claims: claimList.ToArray()
                );
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult()
                {
                    IsError = true,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<List<Claim>> ValidateUserAsync(string openId, string unionId)
        {
            // 数据库查询
            var user = new ApplicationUser();

            await Task.Run(() =>
            {
                // TODO
            });

            if (user == null)
            {
                //注册用户
            }

            return new List<Claim>()
            {
                new Claim(ClaimTypes.Name, $"{openId}"),
            };
        }
    }
}
