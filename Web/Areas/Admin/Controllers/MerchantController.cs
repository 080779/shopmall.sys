using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Merchant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class MerchantController : Controller
    {
        public IPlatformUserService platformUserService { get; set; }
        public IAdminService adminService { get; set; }
        public IJournalService journalService { get; set; }
        private int pageSize = 10;
        [Permission("商家管理_商家管理")]
        public ActionResult List()
        {
            return View();
        }
        [Permission("商家管理_商家管理")]
        [HttpPost]
        public async Task<ActionResult> List(string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            long userId = Convert.ToInt64(Session["Platform_User_Id"]);
            var result = await platformUserService.GetModelListAsync(mobile, code, "商家", startTime, endTime, pageIndex, pageSize);
            var user = await platformUserService.GetModelAsync(userId);
            ListViewModel model = new ListViewModel();
            model.PlatformUsers = result.PlatformUsers;
            model.PlatformIntegral = user.PlatformIntegral;

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
        [Permission("商家管理_商家管理")]
        [Permission("商家管理_添加商家")]
        [AdminLog("商家管理", "添加商家")]
        public async Task<ActionResult> Add(string mobile, string code, string password,string tradePassword)
        {
            string adminMobile = (await adminService.GetModelAsync(Convert.ToInt64(Session["Platform_AdminUserId"]))).Mobile;
            long userId = (await platformUserService.GetModelAsync("mobile", adminMobile)).Id;
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商家账号不能为空" });
            }
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "会员编号不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "登录密码不能为空" });
            }
            if (string.IsNullOrEmpty(tradePassword))
            {
                return Json(new AjaxResult { Status = 0, Msg = "交易密码不能为空" });
            }
            if (await platformUserService.IsExist("mobile", mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商家账号已经存在" });
            }
            if (await platformUserService.IsExist("code", code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "会员编号已经存在" });
            }
            if ((await platformUserService.AddAsync(userId, "商家", mobile, code, password, tradePassword)) <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加商家失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加商家成功" });
        }
        [Permission("商家管理_商家管理")]
        [Permission("商家管理_删除商家")]
        [AdminLog("商家管理", "删除商家")]
        public async Task<ActionResult> Del(long id)
        {
            if (!await platformUserService.DelAsync(id))
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除成功" });
        }
        [Permission("商家管理_商家管理")]
        [Permission("商家管理_冻结商家")]
        [AdminLog("商家管理", "冻结商家")]
        public async Task<ActionResult> Frozen(long id)
        {
            if (!await platformUserService.Frozen(id))
            {
                return Json(new AjaxResult { Status = 0, Msg = "冻结、解冻商家账户操作失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻商家账户操作成功" });
        }
        [Permission("商家管理_商家管理")]
        [Permission("商家管理_发放积分")]
        [AdminLog("商家管理", "发放积分")]
        public async Task<ActionResult> Provide(long toUserId, string strIntegral, string typeName, string tip)
        {
            string adminMobile = (await adminService.GetModelAsync(Convert.ToInt64(Session["Platform_AdminUserId"]))).Mobile;
            long userId = (await platformUserService.GetModelAsync("mobile", adminMobile)).Id;
            if (string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "发放积分额度不能为空" });
            }
            long integral;
            if (!long.TryParse(strIntegral, out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请正确输入发放积分额度" });
            }
            if (integral < 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "请发放积分额度必须大于零" });
            }
            var toUser = await platformUserService.GetModelAsync(toUserId);
            if (toUser.IsEnabled == false)
            {
                return Json(new AjaxResult { Status = 0, Msg = "商家账户已经被冻结" });
            }
            var res = await platformUserService.ProvideAsync(userId, toUserId, integral, "平台积分", typeName, "平台发放", tip);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "发放失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "发放成功" });
        }
        [Permission("商家管理_商家管理")]
        [Permission("商家管理_扣除积分")]
        [AdminLog("商家管理", "扣除积分")]
        public async Task<ActionResult> TakeOut(long toUserId, string strIntegral, string tip, string typeName="消费积分")
        {
            string adminMobile = (await adminService.GetModelAsync(Convert.ToInt64(Session["Platform_AdminUserId"]))).Mobile;
            long userId = (await platformUserService.GetModelAsync("mobile", adminMobile)).Id;
            if (string.IsNullOrEmpty(strIntegral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "发放积分额度不能为空" });
            }
            long integral;
            if (!long.TryParse(strIntegral, out integral))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请正确输入发放积分额度" });
            }
            if (integral <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "发放积分额度必须大于零" });
            }
            var toUser = await platformUserService.GetModelAsync(toUserId);
            if (toUser.IsEnabled == false)
            {
                return Json(new AjaxResult { Status = 0, Msg = "商家账户已经被冻结" });
            }
            if (typeName == "商家积分")
            {
                if (integral > toUser.GivingIntegral)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "积分不足" });
                }
            }
            if (typeName == "消费积分")
            {
                if (integral > toUser.UseIntegral)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "积分不足" });
                }
            }
            if (string.IsNullOrEmpty(typeName))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请选择积分类型" });
            }
            var res = await platformUserService.TakeOutAsync(userId,toUserId, integral, typeName, "平台扣除",tip);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "扣除失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "扣除成功" });
        }
        [Permission("商家管理_商家管理")]
        public async Task<ActionResult> GetIntegral(long toUserId, string typeName)
        {
            var res = await platformUserService.GetModelAsync(toUserId);
            long integral;
            if (typeName == "商家积分")
            {
                integral = res.GivingIntegral;
            }
            else
            {
                integral = res.UseIntegral;
            }
            return Json(new AjaxResult { Status = 1, Data = integral });
        }
        [Permission("商家管理_商家管理")]
        [Permission("商家管理_修改密码")]
        [AdminLog("商家管理", "修改密码")]
        public async Task<ActionResult> EditPwd(long id, string password,string typeName)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "新密码不能为空" });
            }
            if (password.Length < 6 || password.Length > 8)
            {
                return Json(new AjaxResult { Status = 0, Msg = "新密码要6-8位" });
            }
            if(string.IsNullOrEmpty(typeName))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请选择密码类型" });
            }
            //var toUser = await platformUserService.GetModelAsync(id);
            //if (toUser.IsEnabled == false)
            //{
            //    return Json(new AjaxResult { Status = 0, Msg = "商家账户已经被冻结" });
            //}
            bool res;
            if (typeName=="登录密码")
            {
                res = await platformUserService.UpdatePwdAsync(id, password);
            }
            else if (typeName == "支付密码")
            {
                res = await platformUserService.UpdateTradePwdAsync(id, password);
            }
            else
            {
                res = false;
            }
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改成功" });
        }
        [Permission("商家管理_商家管理")]
        public async Task<ActionResult> GetJournal(long id,int pageIndex=1)
        {
            JournalSearchResult result = await journalService.GetMerchantModelListAsync(id, null, null, null, null, null, pageIndex, pageSize);
            GetJournalViewModel model = new GetJournalViewModel();
            model.Journals = result.Journals;

            Pagination pager = new Pagination();
            pager.PageIndex = pageIndex;
            pager.PageSize = pageSize;
            pager.TotalCount = result.TotalCount;
            pager.GetPagerHtml();

            model.Pages = pager.Pages;
            model.PageCount = pager.PageCount;
            return Json(new AjaxResult { Status = 1, Data = model });
        }
    }
}