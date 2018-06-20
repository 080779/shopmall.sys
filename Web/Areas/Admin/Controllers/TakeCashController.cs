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
        public IStateService stateService { get; set; }
        public IPlatformUserService platformUserService { get; set; }
        private int pageSize = 10;
        [Permission("积分管理_积分管理")]
        public ActionResult List()
        {
            return View();
        }
        [Permission("积分管理_积分管理")]
        [HttpPost]
        public async Task<ActionResult> List(long? stateId, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            //long? typeId = await journalTypeService.GetIdByDescAsync("赠送");
            TakeCashSearchResult result = await takeCashService.GetModelListAsync(stateId,mobile,startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.TakeCashes = result.TakeCashes;
            model.States = await stateService.GetModelListAsync();
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
            //return View(model);
        }
        [Permission("积分管理_积分管理")]
        [Permission("积分管理_积分变现")]
        [AdminLog("积分管理", "积分变现")]
        public async Task<ActionResult> TakeCash(string mobile,string strIntegral,string type="消费积分")
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不能为空" });
            }
            var user= await  platformUserService.GetModelAsync("mobile", mobile);
            if(user==null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不存在" });
            }
            if(string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须填" });
            }
            long integral;
            if (!long.TryParse(strIntegral, out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须是数字" });
            }
            if(integral<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须大于零" });
            }
            if (user.IsEnabled == false)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账户已经被冻结" });
            }
            if (type=="消费积分")
            {
                if(integral>user.UseIntegral)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "消费积分不足" });
                }
            }
            if(type=="商家积分")
            {
                if (integral > user.GivingIntegral)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "商家积分不足" });
                }
            }
            bool res= await platformUserService.TakeCashApplyAsync(user.Id, integral, type, "积分变现");
            if(!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现申请失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "变现申请成功" });
        }
        [Permission("积分管理_积分管理")]

        public async Task<ActionResult> Calc(string mobile, string strIntegral, string type = "消费积分")
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不能为空" });
            }
            var user = await platformUserService.GetModelAsync("mobile", mobile);
            if (user == null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不存在" });
            }
            if (string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须填" });
            }
            long integral;
            if (!long.TryParse(strIntegral, out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须是数字" });
            }
            if (integral < 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "变现积分数量必须大于零" });
            }
            decimal amount=0;
            if(type=="消费积分")
            {
                if (integral > user.UseIntegral)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "账户积分不足" });
                }
                amount = await takeCashService.CalcAsync("消费积分提现比率", integral);
            }
            if (type == "商家积分")
            {
                if(integral > user.GivingIntegral)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "账户积分不足" });
                }
                amount = await takeCashService.CalcAsync("商家积分提现比率", integral);
            }
            return Json(new AjaxResult { Status = 1, Data= amount });
        }
        [Permission("积分管理_积分管理")]
        public async Task<ActionResult> GetIntegral(string mobile,string type = "消费积分")
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不能为空" });
            }
            var user = await platformUserService.GetModelAsync("mobile", mobile);
            if (user == null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不存在" });
            }
            long haveIntegral = 0;
            if (type == "消费积分")
            {
                haveIntegral = user.UseIntegral;
            }
            if (type == "商家积分")
            {
                haveIntegral = user.GivingIntegral;
            }
            return Json(new AjaxResult { Status = 1, Data = haveIntegral });
        }
        [Permission("积分管理_积分管理")]
        [Permission("积分管理_确认转账")]
        [AdminLog("积分管理", "确认转账")]
        public async Task<ActionResult> Confirm(long id)
        {
            var res= await platformUserService.TakeCashConfirmAsync(id);
            if(res<=0)
            {
                if(res==-3)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "积分已经不足以提现" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "确认转账失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "确认转账状态修改成功" });
        }
        public async Task<ActionResult> Cancel(long id)
        {
            var res = await platformUserService.TakeCashCancelAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "取消转账失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "取消转账成功" });
        }
    }
}