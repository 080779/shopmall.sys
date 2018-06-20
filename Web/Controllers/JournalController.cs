using IMS.Common;
using IMS.IService;
using IMS.Web.Models.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class JournalController : Controller
    {
        public IJournalService journalService { get; set; }
        public IJournalTypeService journalTypeService { get; set; }
        public IPlatformUserService platformUserService { get; set; }
        private int pageSize = 10;
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List( long? typeId,string mobile,string code, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            long id = Convert.ToInt64(Session["Merchant_User_Id"]);
            var result = await journalService.GetMerchantModelListAsync(id, typeId, mobile, code, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.Journals = result.Journals;
            model.JournalTypes = await journalTypeService.GetModelListAsync("商家");
            var user = await platformUserService.GetModelAsync(id);
            model.GivingIntegral = user.GivingIntegral;
            model.UseIntegral = user.UseIntegral;

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