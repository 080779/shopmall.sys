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
        [HttpPost]
        public async Task<ActionResult> List(long? stateId, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            //long? typeId = await journalTypeService.GetIdByDescAsync("赠送");
            TakeCashSearchResult result = await takeCashService.GetModelListAsync(stateId,mobile,startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.TakeCashes = result.TakeCashes;
            model.States = await idNameService.GetByTypeNameAsync("提现状态");
            if(result.GivingIntegralCount==null)
            {
                result.GivingIntegralCount = 0;
            }
            if (result.UseIntegralCount == null)
            {
                result.UseIntegralCount = 0;
            }
            model.GivingIntegralCount = result.GivingIntegralCount;
            model.UseIntegralCount = result.UseIntegralCount;

            Pagination pager = new Pagination();
            pager.PageIndex = pageIndex;
            pager.PageSize = pageSize;
            pager.TotalCount = result.TotalCount;

            if (result.TotalCount <= pageSize)
            {
                model.PageHtml = "test";
            }
            else
            {
                model.PageHtml = pager.GetPagerHtml();
            }
            model.Pages = pager.Pages;
            model.PageCount = pager.PageCount;
            return Json(new AjaxResult { Status = 1, Data = model });
        }        
    }
}