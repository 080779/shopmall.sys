using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Journal;
using IMS.Web.Areas.Admin.Models.TakeCash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class JournalController : Controller
    {
        public IJournalService journalService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IUserService userService { get; set; }
        public IOrderService orderService { get; set; }
        private int pageSize = 10;
        //[Permission("积分管理_积分管理")]
        public ActionResult List()
        {
            return View();
        }
        [Permission("佣金记录_查看记录")]
        [AdminLog("佣金记录", "查看佣金记录列表")]
        [HttpPost]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            await orderService.AutoConfirmAsync();
            long journalTypeId = await idNameService.GetIdByNameAsync("佣金收入");
            JournalSearchResult result = await journalService.GetModelListAsync(null,journalTypeId, keyword, startTime, endTime, pageIndex, pageSize);
            CalcAmountResult res = await userService.CalcCount();
            ListViewModel model = new ListViewModel();
            model.Journals = result.Journals;
            model.PageCount = result.PageCount;
            model.TotalAmount = res.TotalAmount;
            model.TotalTakeCash = res.TotalTakeCash;
            model.TotalBuyAmount = res.TotalBuyAmount;
            return Json(new AjaxResult { Status = 1, Data = model });
        }        
    }
}