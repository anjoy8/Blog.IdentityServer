using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.IdentityServer.Extensions
{
    /// <summary>
    /// 自定义授权类型
    /// </summary>
    public class GrantTypeCustom
    {
        /// <summary>
        /// GrantType - 自定义微信授权
        /// </summary>
        public const string ResourceWeixinOpen = "weixinopen";
    }
}
