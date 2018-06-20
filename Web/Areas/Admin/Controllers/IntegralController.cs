using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Integral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class IntegralController : Controller
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
        [Permission("积分管理_积分管理")]
        [HttpPost]
        public async Task<ActionResult> List(long? typeId,string mobile,string code, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            JournalSearchResult result = await journalService.GetIntegralModelListAsync(typeId, mobile, code, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.Journals = result.Journals;
            model.JournalTypes = await journalTypeService.GetModelListAsync(true);
            model.GivingIntegralCount = result.GivingIntegrals==null?0: result.GivingIntegrals;
            model.UseIntegralCount = result.UseIntegrals==null?0: result.UseIntegrals;
            model.PlatformIntegral = result.PlatformIntegral;

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
        [Permission("积分管理_积分管理")]
        [Permission("积分管理_增加积分")]
        [AdminLog("积分管理", "增加积分")]
        public async Task<ActionResult> Add(string strIntegral,string tip)
        {
            long userId = Convert.ToInt64(Session["Platform_User_Id"]);
            long integral;
            if(string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "增加额度不能为空" });
            }
            if(!long.TryParse(strIntegral,out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请正确输入增加额度" });
            }
            if(integral<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "增加额度必须大于零" });
            }
            bool res= await platformUserService.ProvideAsync(userId, userId, integral, "平台积分", "平台积分", "积分增加", tip);
            if(!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "增加积分失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg="增加积分成功"});
        }
    }
}