using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using System;

namespace Blog.IdentityServer.Extensions
{
    public static class IpLimitMildd
    {
        public static void UseIpLimitMildd(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                app.UseIpRateLimiting();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
