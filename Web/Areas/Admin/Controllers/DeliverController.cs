using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Deliver;
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
        [AdminLog("发货管理", "查看收发货列表")]
        public async Task<ActionResult> List(long? orderStateId,string keyword,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            var result = await orderService.GetDeliverModelListAsync(null,orderStateId, keyword, startTime, endTime, pageIndex, pageSize);
            DeliverListViewModel res = new DeliverListViewModel();
            res.Orders = result.Orders;
            res.PageCount = result.PageCount;
            List<IdNameDTO> lists = new List<IdNameDTO>();
            lists.Add(await idNameService.GetByNameAsync("待发货"));
            lists.Add(await idNameService.GetByNameAsync("已发货"));
            res.OrderStates = lists;
            return Json(new AjaxResult { Status = 1, Data = res });
        }
        public async Task<ActionResult> GetModel(long id)
        {
            var res = await orderService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = res });
        }
        [AdminLog("发货管理", "标记发货")]
        public async Task<ActionResult> Edit(long id,string deliverName,string deliverCode)
        {
            bool flag = await orderService.UpdateDeliverStateAsync(id, deliverName, deliverCode);
            if(!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "标记发货失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "标记发货成功" });
        }
    }
}