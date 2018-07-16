using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 发货管理
    /// </summary>
    public class DeliverController : Controller
    {
        public IOrderService orderService { get; set; }
        public IIdNameService idNameService { get; set; }
        private int pageSize = 10;
        //[Permission("日志管理_查看日志")]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        //[Permission("日志管理_查看日志")]
        public async Task<ActionResult> List(string keyword,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            long orderStateId = await idNameService.GetIdByNameAsync("");
            var result = await orderService.GetModelListAsync(null,keyword, startTime, endTime, pageIndex, pageSize,null);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
    }
}