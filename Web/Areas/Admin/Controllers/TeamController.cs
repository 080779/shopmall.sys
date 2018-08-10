using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class TeamController : Controller
    {
        private int pageSize = 10;
        public IUserService userService { get; set; }
        public IIdNameService idNameService { get; set; }
        public ISettingService settingService { get; set; }
        //public IOrderService orderService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("团队管理", "查看团队管理列表")]
        //[Permission("幻灯片管理_删除幻灯片")]
        public async Task<ActionResult> List(long? levelId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            //await orderService.AutoConfirmAsync();
            var result = await userService.GetModelListAsync(levelId, keyword, startTime, endTime, pageIndex, pageSize);
            TeamUserListViewModel model = new TeamUserListViewModel();
            model.PageCount = result.PageCount;
            model.Users = result.Users;
            model.Levels = await idNameService.GetByTypeNameAsync("会员等级");
            model.TeamLevels = await settingService.GetModelListAsync("代理等级");
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        public ActionResult TeamList()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("团队管理", "查看团队管理列表")]
        //[Permission("幻灯片管理_删除幻灯片")]
        public async Task<ActionResult> TeamList(string mobile, long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var res = await userService.GetModelTeamListAsync(mobile, teamLevel, keyword, startTime, endTime, pageIndex, pageSize);
            TeamListViewModel model = new TeamListViewModel();
            model.PageCount = res.PageCount;
            model.TotalCount = res.TotalCount;
            model.Members = res.Members;
            model.TeamLevels = await settingService.GetModelListAsync("代理等级");
            model.TeamLeader = res.TeamLeader;
            return Json(new AjaxResult { Status = 1, Data = model });
        }
    }
}