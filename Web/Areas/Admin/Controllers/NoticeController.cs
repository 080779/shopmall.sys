using IMS.Common;
using IMS.IService;
using IMS.Web.Areas.Admin.Models.Notice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class NoticeController : Controller
    {
        private int pageSize = 1;
        public INoticeService noticeService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await noticeService.GetModelListAsync(mobile, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.Notices = result.Notices;
            model.PageResult = PageHelper.GetPagerHtml(result.TotalCount, pageSize, pageIndex);

            return Json(new AjaxResult { Status = 1, Data = model });
        }
        public ActionResult List1()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List1(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await noticeService.GetModelListAsync(mobile, startTime, endTime, pageIndex, pageSize);
            //ListViewModel model = new ListViewModel();
            //model.Notices = result.Notices;
            //model.PageResult = PageHelper.GetPagerHtml(result.TotalCount, pageSize, pageIndex);

            return Json(new AjaxResult { Status = 1, Data = result });
        }
    }
}