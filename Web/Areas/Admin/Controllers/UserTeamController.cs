using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.User;
using IMS.Web.Areas.Admin.Models.UserTeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class UserTeamController : Controller
    {
        private int pageSize = 10;
        public IUserService userService { get; set; }
        public IIdNameService idNameService { get; set; }
        public ISettingService settingService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(string mobile,long? userId,long? teamLevel,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            if(!string.IsNullOrEmpty(mobile))
            {
                mobile= await settingService.GetParmByNameAsync("系统会员");
            }
            UserDTO user= await userService.GetModelByMobileAsync(mobile);
            var res = await userService.GetModelTeamListAsync(user.Id, teamLevel, keyword, startTime, endTime, pageIndex, pageSize);
            UserTeamListViewModel model = new UserTeamListViewModel();
            model.PageCount = res.PageCount;
            model.Members = res.Users;
            model.TeamLevels = await settingService.GetModelListAsync("会员等级");
            model.Header = user;
            return Json(new AjaxResult { Status = 1, Data = model });
        }        
    }
}