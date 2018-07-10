﻿using IMS.Common;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IMS.Web.Areas.Admin.Models.Order;
using IMS.DTO;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private int pageSize = 10;
        public IOrderService orderService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IOrderListService orderListService { get; set; }
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
        public ActionResult Detail(long id)
        {
            return View(id);
        }

        [HttpPost]
        public async Task<ActionResult> GetDetail(long id)
        {
            OrderDTO dto= await orderService.GetModelAsync(id);
            OrderListSearchResult result = await orderListService.GetModelListAsync(dto.Id, null, null, null, 1, 100);
            OrderDetailViewModel model = new OrderDetailViewModel();
            model.BasicInfo = new BasicInfo { Amount=dto.Amount,Code=dto.Code,CreateTime=dto.CreateTime,OrderStateName=dto.OrderStateName,PayTypeName=dto.PayTypeName };
            model.BuyerInfo = new BuyerInfo { Address = dto.ReceiverAddress, BuyerMobile = dto.BuyerMobile, Mobile = dto.ReceiverMobile, Name = dto.ReceiverName };
            model.OrderGoodsInfos = result.OrderLists.Select(g=>new OrderGoodsInfo { Code=g.GoodsCode,Id=g.GoodsId,Name=g.GoodsName,Number=g.Number,RealityPrice=g.Price,Thumb=g.ImgUrl }).ToList();
            return Json(new AjaxResult { Status = 1, Data = model });
        }
    }
}