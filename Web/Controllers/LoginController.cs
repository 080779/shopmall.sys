using IMS.Common;
using IMS.IService;
using IMS.Web.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        public IPlatformUserService platformUserService { get; set; }
        [HttpGet]
        [Route("login")]
        public ActionResult Login()
        {
            return View();
        }
        [Route("login")]
        [HttpPost]
        public async Task<ActionResult> Login(string mobile,string password,bool ischecked=false)
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "密码不能为空" });
            }
            long result = await platformUserService.CheckLoginAsync(mobile, password);
            if (result == -1)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号或密码错误" });
            }
            if (result == -2)
            {
                return Json(new AjaxResult { Status = 0, Msg = "该账号不是商家账号" });
            }
            if (result == -3)
            {
                return Json(new AjaxResult { Status = 0, Msg = "输入密码错误次数超过5次，账号已经被冻结，请联系平台解冻" });
            }
            if (result == -4)
            {
                return Json(new AjaxResult { Status = 0, Msg = "该账号已经被冻结" });
            }
            if (result == -5)
            {
                return Json(new AjaxResult { Status = 0, Msg = "账号或密码错误" });
            }
            Session["Merchant_User_Id"] = result;
            return Json(new AjaxResult { Status = 302, Msg = "登录成功", Data = "/home/index" });
        }
        [Route("logout")]
        public ActionResult Logout()
        {
            Session["Merchant_User_Id"] = null;
            return Redirect("/login");
        }
    }
}