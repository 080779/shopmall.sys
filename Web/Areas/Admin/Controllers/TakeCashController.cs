using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.TakeCash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class TakeCashController : Controller
    {
        public ITakeCashService takeCashService { get; set; }
        public IIdNameService idNameService { get; set; }
        private int pageSize = 10;
        //[Permission("积分管理_积分管理")]
        public ActionResult List()
        {
            return View();
        }
        //[Permission("积分管理_积分管理")]
        [AdminLog("佣金结款", "查看佣金结款管理列表")]
        [HttpPost]
        public async Task<ActionResult> List(long? stateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            TakeCashSearchResult result = await takeCashService.GetModelListAsync(null,stateId, keyword, startTime, endTime, pageIndex, pageSize);
            TakeCashListViewModel model = new TakeCashListViewModel();
            model.TakeCashes = result.TakeCashes;
            model.PageCount = result.PageCount;
            model.States = await idNameService.GetByTypeNameAsync("提现状态");
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [HttpPost]
        [AdminLog("佣金结款", "确认结款")]
        [Permission("佣金结款_标记结款")]
        public async Task<ActionResult> Confirm(long id)
        {
            long res = await takeCashService.Confirm(id, Convert.ToInt64(Session["Platform_AdminUserId"]));
            if(res<=0)
            {
                if(res==-3)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "账户余额不足,结款失败" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "结款失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "结款成功" });
        }
    }
}