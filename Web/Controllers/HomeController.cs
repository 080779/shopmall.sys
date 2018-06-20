using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class HomeController : Controller
    {
        public IPlatformUserService platformUserService { get; set; }
        public IJournalService journalService { get; set; }
        public async Task<ActionResult> Index()
        {
            long id = Convert.ToInt64(Session["Merchant_User_Id"]);
            var user= await platformUserService.GetModelAsync(id);
            return View((object)user.Mobile);
        }
        public ActionResult Home()
        {
            return View();
        }
    }
}