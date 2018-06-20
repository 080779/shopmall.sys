using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Journal;
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
        public IJournalTypeService journalTypeService { get; set; }
        public IPlatformUserService platformUserService { get; set; }
        private int pageSize = 10;
        [Permission("积分管理_积分管理")]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [Permission("积分管理_积分管理")]
        public async Task<ActionResult> List(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var user = await platformUserService.GetModelAsync("mobile", "PlatformUser201805051709360001");
            //long? typeId = await journalTypeService.GetIdByDescAsync("赠送");
            JournalSearchResult result = await journalService.GetModelListAsync("交易", mobile, null, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.Journals = result.Journals;
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
            //return View(model);
        }
    }
}