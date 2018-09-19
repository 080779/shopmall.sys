using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.User;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class TestController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(TestController));
        public ActionResult List()
        {            
            return View();
        }

        public ActionResult Index()
        {
            try
            {
                log.DebugFormat("传进来的code：{DateTime.Now}");
            }
            catch(Exception ex)
            {
                log.DebugFormat("错误：{ex.ToString()}");
            }
            return View();
        }

        public ActionResult Code()
        {
            string verifyCode = CommonHelper.GetCaptcha(4);
            TempData["verifyCode"] = verifyCode;
            VerificationCode code = new VerificationCode();
            MemoryStream ms = code.OutputImageStream(verifyCode);
            return File(ms, "image/jpeg");
        }
        public ActionResult getres()
        {
            return Json(new AjaxResult { Status = 1 ,Data=new listres() });
        }
        public class file
        {
            public string src { get; set; }
        }
        public class listres
        {
            public List<file> imgList { get; set; }
        }
    }
}