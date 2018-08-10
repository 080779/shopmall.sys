using IMS.IService;
using IMS.Web.Areas.Admin.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IAdminService adminService { get; set; }
        public ISettingService settingService { get; set; }
        public IPermissionTypeService permissionTypeService { get; set; }
        public IOrderService orderService { get; set; }
        public async Task<ActionResult> Index()        {

            long userId = Convert.ToInt64(Session["Platform_AdminUserId"]);
            HomeIndexViewModel model = new HomeIndexViewModel();
            model.Mobile = (await adminService.GetModelAsync(userId)).Mobile;
            model.PermissionTypes = await permissionTypeService.GetModelList();
            model.SysTitle= await settingService.GetParmByNameAsync("系统标题");
            return View(model);
        }
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Permission(string msg)
        {
            return View((object)msg);
        }
    }
}