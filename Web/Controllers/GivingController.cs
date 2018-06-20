using IMS.Common;
using IMS.IService;
using IMS.Web.Models.Giving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class GivingController : Controller
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
        public async Task<ActionResult> List(string mobile,string code,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            long id = Convert.ToInt64(Session["Merchant_User_Id"]);
            var user= await platformUserService.GetModelAsync(id);
            var result = await journalService.GetGivingModelListAsync(id, mobile, code, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.Journals = result.Journals;
            model.HaveGivingIntegral = user.GivingIntegral;

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

        public async Task<ActionResult> Provide(string mobile,string strIntegral,string tip)
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "赠送账户不能为空" });
            }
            if (string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "赠送积分额度不能为空" });
            }
            long integral;
            if(!long.TryParse(strIntegral,out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请输入正确的赠送额度" });
            }
            if(integral<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "赠送积分额度必须大于零" });
            }
            long id = Convert.ToInt64(Session["Merchant_User_Id"]);
            var toUser= await platformUserService.GetModelAsync("mobile", mobile);
            if(toUser==null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "赠送客户账号不存在" });
            }
            if(toUser.PlatformUserTypeName=="平台")
            {
                return Json(new AjaxResult { Status = 0, Msg = "请填写其他客户账号" });
            }
            if(toUser.IsEnabled==false)
            {
                return Json(new AjaxResult { Status = 0, Msg = "赠送客户账号已经被冻结" });
            }
            var user = await platformUserService.GetModelAsync(id);
            if (user.GivingIntegral<integral)
            {
                return Json(new AjaxResult { Status = 0, Msg = "积分不足" });
            }
            if(toUser.Id==id)
            {
                return Json(new AjaxResult { Status = 0, Msg = "请填写其他客户账号" });
            }
            await platformUserService.ProvideAsync(id, toUser.Id, integral, "商家积分", "消费积分", "赠送", tip);
            return Json(new AjaxResult { Status = 1, Msg="赠送成功" });
        }
    }
}