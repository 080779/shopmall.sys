using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class SettingController : Controller
    {
        public ISettingService settingService { get; set; }
        private int pageSize = 10;
        //[Permission("日志管理_查看日志")]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        //[Permission("日志管理_查看日志")]
        public async Task<ActionResult> List(string keyword,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            SettingListViewModel model = new SettingListViewModel();
            var tilte= await settingService.GetModelByNameAsync("系统标题");
            model.Title = new SettingModel { Id = tilte.Id, Parm = tilte.Parm };
            var phone1 = await settingService.GetModelByNameAsync("客服电话");
            model.Phone1 = new SettingModel { Id = phone1.Id, Parm = phone1.Parm };
            var phone2 = await settingService.GetModelByNameAsync("客服电话1");
            model.Phone2 = new SettingModel { Id = phone2.Id, Parm = phone2.Parm };
            var logo = await settingService.GetModelByNameAsync("系统LOGO");
            model.Logo = new SettingModel { Id = logo.Id, Parm = logo.Parm };
            var about = await settingService.GetModelByNameAsync("关于我们");
            model.About = new SettingModel { Id = about.Id, Parm = about.Parm };
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(List<SettingModel> parms)
        {
            return Json(new AjaxResult { Status = 1});
        }
    }
}