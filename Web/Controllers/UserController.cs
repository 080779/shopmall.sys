using IMS.Common;
using IMS.IService;
using IMS.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class UserController : Controller
    {
        public IPlatformUserService platformUserService { get; set; }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Add(string mobile,string code,string password)
        {
            long id = Convert.ToInt64(Session["Merchant_User_Id"]);
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg="客户账号不能为空"});
            }
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "会员编号不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "交易密码不能为空" });
            }
            if(await platformUserService.IsExist("mobile",mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号已经存在" });
            }
            if (await platformUserService.IsExist("code", code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "会员编号已经存在" });
            }
            if(await platformUserService.AddAsync(id,"客户", mobile, code, "", password)<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加客户失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg= "添加客户成功" });
        }

        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Search(string type,string code, string password)
        {
            if(string.IsNullOrEmpty(type))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请选择类型" });
            }
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号或会员编号不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "支付密码不能为空" });
            }
            var user = await platformUserService.GetModelAsync(type, code);
            if (user==null)
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号或会员编号不存在" });
            }
            if(!user.IsEnabled)
            {
                return Json(new AjaxResult { Status = 0, Msg = "客户账号已经被冻结" });
            }
            if (user.PlatformUserTypeName == "平台")
            {
                return Json(new AjaxResult { Status = 0, Msg = "没有权限" });
            }
            if (!await platformUserService.CheckTradePasswordAsync(user.Id,password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "支付密码错误" });
            }
            SearchViewModel model = new SearchViewModel();
            model.Mobile = user.Mobile;
            model.Code = user.Code;
            model.Integral = user.UseIntegral;
            return Json(new AjaxResult { Status = 1, Msg = "查询成功", Data = model });
        }
    }
}