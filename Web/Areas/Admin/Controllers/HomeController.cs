using IMS.IService;
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
        public async Task<ActionResult> Index()
        {
            long userId = Convert.ToInt64(Session["Platform_AdminUserId"]);
            var user= await adminService.GetModelAsync(userId);
            return View((object)user.Mobile);
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