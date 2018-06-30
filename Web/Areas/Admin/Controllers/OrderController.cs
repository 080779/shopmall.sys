using IMS.Common;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IMS.Web.Areas.Admin.Models.Order;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private int pageSize = 10;
        public IOrderService orderService { get; set; }
        public IIdNameService idNameService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(long? orderStateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await orderService.GetModelListAsync(null, orderStateId, keyword, startTime, endTime, pageIndex, pageSize);
            OrderListViewModel model = new OrderListViewModel();
            model.Orders = result.Orders;
            model.PageCount = result.PageCount;
            model.OrderStates = (await idNameService.GetByTypeNameAsync("订单状态"));
            return Json(new AjaxResult { Status = 1, Data = model });
        }
    }
}