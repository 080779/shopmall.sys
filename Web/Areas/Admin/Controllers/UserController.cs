using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private int pageSize = 10;
        public IUserService userService { get; set; }
        public IIdNameService idNameService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(long? levelId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            //long levelId = await idNameService.GetIdByNameAsync("会员等级");
            var result = await userService.GetModelListAsync(levelId, keyword, startTime, endTime, pageIndex, pageSize);
            UserListViewModel model = new UserListViewModel();
            model.PageCount = result.PageCount;
            model.Users = result.Users;
            model.Levels = await idNameService.GetByTypeNameAsync("会员等级");
            return Json(new AjaxResult { Status = 1, Data = model });
        }
    }
}