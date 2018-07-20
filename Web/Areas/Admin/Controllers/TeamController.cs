using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.UserTeam;
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
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("团队管理", "查看团队管理列表")]
        //[Permission("幻灯片管理_删除幻灯片")]
        public async Task<ActionResult> List(string mobile, long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var res = await userService.GetModelTeamListAsync(mobile, teamLevel, keyword, startTime, endTime, pageIndex, pageSize);
            UserTeamListViewModel model = new UserTeamListViewModel();
            model.PageCount = res.PageCount;
            model.Members = res.Members;
            model.TeamLevels = await settingService.GetModelListAsync("代理等级");
            model.TeamLeader = res.TeamLeader;
            return Json(new AjaxResult { Status = 1, Data = model });
        }
    }
}