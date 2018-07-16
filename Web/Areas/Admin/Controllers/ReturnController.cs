using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 退货管理
    /// </summary>
    public class ReturnController : Controller
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
        public async Task<ActionResult> List(long? auditStatusId, string keyword,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            long orderStateId = await idNameService.GetIdByNameAsync("退货中");
            var result = await orderService.GetModelListAsync(null, orderStateId, auditStatusId, keyword, startTime, endTime, pageIndex, pageSize);
            ReturnListViewModel model = new ReturnListViewModel();
            model.Orders = result.Orders;
            model.PageCount = result.PageCount;
            model.AuditStatus = await idNameService.GetByTypeNameAsync("退货审核状态");
            return Json(new AjaxResult { Status = 1, Data = model });
        }
    }
}