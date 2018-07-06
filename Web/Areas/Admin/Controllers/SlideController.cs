using IMS.Common;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class SlideController : Controller
    {
        private int pageSize = 10;
        public ISlideService slideService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await slideService.GetModelListAsync(mobile, startTime, endTime, pageIndex, pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        public async Task<ActionResult> Add(string name, string url, string imgFile, bool isEnabled)
        {
            string res;
            if(!ImageHelper.SaveBase64(imgFile, out res))
            {
                return Json(new AjaxResult { Status = 0, Msg=res});
            }
            long id=  await slideService.AddAsync(name, url, res, isEnabled);
            if(id<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加幻灯片失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加幻灯片成功" });
        }
    }
}