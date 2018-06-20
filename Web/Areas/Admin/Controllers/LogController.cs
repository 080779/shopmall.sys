using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class LogController : Controller
    {
        public IAdminLogService adminLogService { get; set; }
        private int pageSize = 10;
        [Permission("日志管理_查看日志")]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [Permission("日志管理_查看日志")]
        public async Task<ActionResult> List(string mobile,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            var result = await adminLogService.GetModelListAsync(mobile, null, startTime, endTime, pageIndex, pageSize);
            ListViewModel model = new ListViewModel();
            model.AdminLogs = result.AdminLogs;

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

        //public ActionResult List1()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<ActionResult> List1(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        //{
        //    var result = await adminLogService.GetModelListAsync(mobile, null, startTime, endTime, pageIndex, pageSize);
        //    ListViewModel1 model = new ListViewModel1();
        //    model.AdminLogs = result.AdminLogs;
        //    model.PageResult = PageHelper.GetPagerHtml(result.TotalCount, pageSize, pageIndex);

        //    return Json(new AjaxResult { Status = 1, Data = model });
        //}
    }
}