using Blog.IdentityServer.Models;
using Blog.IdentityServer.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.IdentityServer.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class Is4ApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public Is4ApiController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        [HttpGet]
        public MessageModel<AccessApiDateView> GetAchieveUsers()
        {
            List<ApiDate> apiDates = new List<ApiDate>();

            var users = _userManager.Users.Where(d => !d.tdIsDelete).OrderByDescending(d => d.Id).ToList();

            var tadayRegisterUser = users.Where(d => d.birth >= DateTime.Now.Date).Count();

            apiDates = (from n in users
                        group n by new { n.birth.Date } into g
                        select new ApiDate
                        {
                            date = g.Key?.Date.ToString("yyyy-MM-dd"),
                            count = g.Count(),
                        }).ToList();

            apiDates = apiDates.OrderByDescending(d => d.date).Take(30).ToList();


            if (apiDates.Count == 0)
            {
                apiDates.Add(new ApiDate()
                {
                    date = "没数据,或未开启相应接口服务",
                    count = 0
                });
            }
            return new MessageModel<AccessApiDateView>()
            {
                msg = "获取成功",
                success = true,
                response = new AccessApiDateView
                {
                    columns = new string[] { "date", "count" },
                    rows = apiDates.OrderBy(d => d.date).ToList(),
                    today = tadayRegisterUser,
                }
            };
        }
    }
}