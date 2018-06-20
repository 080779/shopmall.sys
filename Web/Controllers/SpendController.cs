using IMS.Common;
using IMS.IService;
using IMS.Web.Models.Spend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class SpendController : Controller
    {
        public IJournalTypeService journalTypeService { get; set; }
        public IPlatformUserService platformUserService { get; set; }
        public IJournalService journalService { get; set; }
        private int pageSize = 10;
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(string mobile,string code,DateTime? startTime,DateTime? endTime,int pageIndex = 1)
        {
            long id = Convert.ToInt64(Session["Merchant_User_Id"]);
            long? typeId = await journalTypeService.GetIdByDescAsync("消费");
            var result = await journalService.GetSpendModelListAsync(id, mobile, code, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.Journals = result.Journals;

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
        [HttpPost]
        public async Task<ActionResult> Spend(string mobile, string strIntegral, string tip,string tradePassword)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号不能为空" });
            }
            if (string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "转出额度不能为空" });
            }
            if(string.IsNullOrEmpty(tradePassword))
            {
                return Json(new AjaxResult { Status = 0, Msg = "交易密码不能为空" });
            }
            long integral;
            if (!long.TryParse(strIntegral, out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "转出额度必须是数字" });
            }
            if (integral <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "转出额度必须大于零" });
            }
            long id = Convert.ToInt64(Session["Merchant_User_Id"]);
            var toUser = await platformUserService.GetModelAsync("mobile", mobile);
            if (toUser == null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号不存在" });
            }
            if (toUser.PlatformUserTypeName == "平台")
            {
                return Json(new AjaxResult { Status = 0, Msg = "请填写其他客户账号" });
            }
            if(toUser.IsEnabled==false)
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号已经被冻结" });
            }
            if (toUser.UseIntegral < integral)
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户积分不足" });
            }
            if (toUser.Id == id)
            {
                return Json(new AjaxResult { Status = 0, Msg = "请填写其他客户账号" });
            }
            if(!(await platformUserService.CheckTradePasswordAsync(toUser.Id,tradePassword)))
            {
                return Json(new AjaxResult { Status = 0, Msg = "交易密码错误" });
            }
            await platformUserService.ProvideAsync(toUser.Id, id, integral, "消费积分", "消费积分", "消费", tip);
            return Json(new AjaxResult { Status = 1, Msg = "转出客户积分成功" });
        }
        public async Task<ActionResult> Check(string mobile,string strIntegral)
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号不能为空" });
            }
            if (string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "转出额度不能为空" });
            }
            long integral;
            if (!long.TryParse(strIntegral, out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "转出额度必须是数字" });
            }
            if (integral <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "转出额度必须大于零" });
            }
            var toUser = await platformUserService.GetModelAsync("mobile", mobile);
            if (toUser == null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号不存在" });
            }
            if (toUser.IsEnabled == false)
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号已经被冻结" });
            }
            return Json(new AjaxResult { Status = 1 });
        }
        public async Task<ActionResult> GetIntegral(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号不能为空" });
            }
            long res;
            if(!long.TryParse(mobile,out res))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请输入正确的客户账号" });
            }
            if(mobile.Length!=11)
            {
                return Json(new AjaxResult { Status = 0, Msg = "请输入正确的客户账号" });
            }
            var result= await platformUserService.GetModelAsync("mobile", mobile);
            if(result==null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号不存在" });
            }
            return Json(new AjaxResult { Status = 1, Data = result.UseIntegral });
        }
    }
}