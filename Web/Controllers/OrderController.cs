﻿using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Goods;
using IMS.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    [AllowAnonymous]
    public class OrderController : ApiController
    {
        public IOrderService orderService { get; set; }
        public IOrderListService orderListService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IUserService userService { get; set; }
        public IOrderApplyService orderApplyService { get; set; }
        public IGoodsService goodsService { get; set; }
        public IGoodsImgService goodsImgService { get; set; }
        public IGoodsCarService goodsCarService { get; set; }
        public ISettingService settingService { get; set; }

        [HttpPost]
        public async Task<ApiResult> List(OrderListModel model)
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            OrderSearchResult result = await orderService.GetModelListAsync(user.Id, model.OrderStateId, null, null, null, null, model.PageIndex, model.PageSize);
            OrderListApiModel res = new OrderListApiModel();
            res.PageCount = result.PageCount;
            res.Orders = result.Orders.Select(o => new Order
            {
                amount = o.Amount,
                code = o.Code,
                createTime = o.CreateTime,
                postFee = o.PostFee,
                deliver = o.Deliver,
                deliveryCode = o.DeliverCode,
                deliveryName = o.DeliverName,
                id = o.Id,
                orderStateId = o.OrderStateId,
                orderStateName = o.OrderStateName,
                payTypeId = o.PayTypeId,
                payTypeName = o.PayTypeName,
                receiverName = o.ReceiverName,
                receiverMobile = o.ReceiverMobile,
                receiverAddress = o.ReceiverAddress,
                payTime = o.PayTime,
                consignTime = o.ConsignTime,
                endTime = o.EndTime,
                closeTime = o.CloseTime,
                discountAmount=o.DeductAmount,
                OrderGoods = orderListService.GetModelList(o.Id).Select(l => new OrderGoods {
                    name = l.GoodsName,
                    number = l.Number,
                    price = l.Price,
                    realityPrice = l.RealityPrice,
                    totalFee = l.TotalFee
                }).ToList()
            }).ToList();
            return new ApiResult { status = 1, data = res };
        }
        [HttpPost]
        public async Task<ApiResult> GoodsList(OrderGoodsListModel model)
        {
            string parm = await settingService.GetParmByNameAsync("网站域名");
            var res = await orderListService.GetModelListAsync(model.OrderId, null, null, null, model.PageIndex, model.PageSize);
            var result = new OrderGoodsListApiModel();
            result.pageCount = res.PageCount;
            result.goodsLists = res.OrderLists.Select(o => new OrderList
            {
                goodsName = o.GoodsName,
                id = o.Id,
                imgUrl = parm + o.ImgUrl,
                isReturn = o.IsReturn,
                number = o.Number,
                orderCode = o.OrderCode,
                orderId = o.OrderId,
                price = o.Price,
                tealityPrice = o.RealityPrice,
                totalFee = o.TotalFee
            });
            return new ApiResult { status = 1, data = result };
        }

        [HttpPost]
        public async Task<ApiResult> GoodsSelect(OrderGoodsSelectModel model)
        {
            await orderListService.ReSetIsReturnAsync(model.OrderId);
            bool flag = await orderListService.SetIsReturnAsync(model.Id);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = "选择出错" };
            }
            return new ApiResult { status = 1, msg = "成功" };
        }

        [HttpPost]
        public async Task<ApiResult> ReturnApply(OrderReturnApplyModel model)
        {
            long id = await orderService.ApplyReturnAsync(model.OrderId);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "退货申请失败" };
            }
            return new ApiResult { status = 1, msg = "退货申请成功" };
        }

        [HttpPost]
        public async Task<ApiResult> Detail(OrderDetailModel model)
        {
            var o = await orderService.GetModelAsync(model.Id);
            if (o == null)
            {
                return new ApiResult { status = 0, msg = "订单不存在" };
            }
            Order order = new Order
            {
                amount = o.Amount,
                code = o.Code,
                createTime = o.CreateTime,
                postFee = o.PostFee,
                deliver = o.Deliver,
                deliveryCode = o.DeliverCode,
                deliveryName = o.DeliverName,
                id = o.Id,
                orderStateId = o.OrderStateId,
                orderStateName = o.OrderStateName,
                payTypeId = o.PayTypeId,
                payTypeName = o.PayTypeName,
                receiverName = o.ReceiverName,
                receiverMobile = o.ReceiverMobile,
                receiverAddress = o.ReceiverAddress,
                payTime = o.PayTime,
                consignTime = o.ConsignTime,
                endTime = o.EndTime,
                closeTime = o.CloseTime,
                OrderGoods = orderListService.GetModelList(o.Id).Select(l => new OrderGoods
                {
                    name = l.GoodsName,
                    number = l.Number,
                    price = l.Price,
                    realityPrice = l.RealityPrice,
                    totalFee = l.TotalFee
                }).ToList()
            };
            return new ApiResult { status = 1, data = order };
        }

        public async Task<ApiResult> Place(OrderPlaceModel model)
        {
            GoodsCarDTO dto = new GoodsCarDTO();
            var goods = await goodsService.GetModelAsync(model.GoodsId);
            if (goods == null)
            {
                return new ApiResult { status = 0, msg = "商品不存在" };
            }
            if (goods.Inventory < model.Number)
            {
                return new ApiResult { status = 0, msg = "商品库存不足" };
            }
            dto.GoodsId = model.GoodsId;
            dto.ImgUrl = goodsImgService.GetFirstImg(model.GoodsId);
            dto.Name = goods.Name;
            dto.Number = model.Number;
            dto.RealityPrice = goods.RealityPrice;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            await orderApplyService.DeleteListAsync(user.Id);
            dto.UserId = user.Id;
            dto.GoodsAmount = dto.RealityPrice * dto.Number;
            long id = await orderApplyService.AddAsync(dto);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "下单失败" };
            }
            return new ApiResult { status = 1, msg = "下单成功" };
        }

        public async Task<ApiResult> Places()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            await orderApplyService.DeleteListAsync(user.Id);
            var res = await goodsCarService.GetModelListAsync(user.Id);
            if (res.Count() <= 0)
            {
                return new ApiResult { status = 0, msg = "购物车没有商品" };
            }
            long id = await orderApplyService.AddAsync(res);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "下单失败" };
            }
            return new ApiResult { status = 1, msg = "下单成功" };
        }

        public async Task<ApiResult> PlaceList()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var res = await orderApplyService.GetModelListAsync(user.Id);
            OrderPlaceListApiModel model = new OrderPlaceListApiModel();
            model.totalAmount = res.ToTalAmount;
            model.orderPlaces = res.OrderApplies.Select(o => new OrderPlace { GoodsId = o.GoodsId, GoodsName = o.GoodsName, ImgUrl = o.ImgUrl, Number = o.Number, Price = o.Price, TotalFee = o.TotalFee, UserId = o.UserId }).ToList();
            return new ApiResult { status = 1, data = model };
        }

        [HttpPost]
        public async Task<ApiResult> Apply(OrderApplyModel model)
        {
            long orderStateId = await idNameService.GetIdByNameAsync("待付款");
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id = await orderService.AddAsync(user.Id, model.AddressId, model.PayTypeId, orderStateId, model.GoodsId, model.Number);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "生成订单失败" };
            }
            long payTypeId = await idNameService.GetIdByNameAsync("余额");
            OrderDTO dto = await orderService.GetModelAsync(id);
            if (payTypeId == model.PayTypeId)
            {
                long payResId = await userService.BalancePayAsync(user.Id, id, dto.Amount);
                if (payResId == -1)
                {
                    return new ApiResult { status = 0, msg = "用户不存在" };
                }
                if (payResId == -2)
                {
                    return new ApiResult { status = 0, msg = "用户账户余额不足" };
                }
                if (payResId == -3)
                {
                    return new ApiResult { status = 0, msg = "订单不存在" };
                }
                orderStateId = await idNameService.GetIdByNameAsync("待发货");
                await orderService.UpdateAsync(id, null, null, orderStateId);
            }
            return new ApiResult { status = 1, msg = "支付成功" };
        }
        [HttpPost]
        public async Task<ApiResult> Applys(OrderApplysModel model)
        {
            long orderStateId = await idNameService.GetIdByNameAsync("待付款");
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var dtos = await orderApplyService.GetModelListAsync(user.Id);
            if (dtos.OrderApplies.Count() <= 0)
            {
                return new ApiResult { status = 0, msg = "下单列表无商品" };
            }
            long id = await orderService.AddAsync(0, user.Id, model.AddressId, model.PayTypeId, orderStateId, dtos.OrderApplies);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "生成订单失败" };
            }
            await goodsCarService.DeleteListAsync(user.Id);
            await orderApplyService.DeleteListAsync(user.Id);
            long payTypeId = await idNameService.GetIdByNameAsync("余额");
            OrderDTO dto = await orderService.GetModelAsync(id);
            if (payTypeId == model.PayTypeId)
            {
                long payResId = await userService.BalancePayAsync(user.Id, id, dto.Amount);
                if (payResId == -1)
                {
                    return new ApiResult { status = 0, msg = "用户不存在" };
                }
                if (payResId == -2)
                {
                    return new ApiResult { status = 0, msg = "用户账户余额不足" };
                }
                if (payResId == -3)
                {
                    return new ApiResult { status = 0, msg = "订单不存在" };
                }
                orderStateId = await idNameService.GetIdByNameAsync("待发货");
                await orderService.UpdateAsync(id, null, null, orderStateId);
            }
            return new ApiResult { status = 1, msg = "支付成功" };
        }

        [HttpPost]
        public async Task<ApiResult> ReApplys(OrderReApplysModel model)
        {
            long orderStateId = 0;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var order = await orderService.GetModelAsync(model.OrderId);
            if (order == null)
            {
                return new ApiResult { status = 0, msg = "订单不存在" };
            }
            long payTypeId = await idNameService.GetIdByNameAsync("余额");
            if (payTypeId == model.PayTypeId)
            {
                long payResId = await userService.BalancePayAsync(user.Id, order.Id, order.Amount);
                if (payResId == -1)
                {
                    return new ApiResult { status = 0, msg = "用户不存在" };
                }
                if (payResId == -2)
                {
                    return new ApiResult { status = 0, msg = "用户账户余额不足" };
                }
                if (payResId == -3)
                {
                    return new ApiResult { status = 0, msg = "订单不存在" };
                }
                orderStateId = await idNameService.GetIdByNameAsync("待发货");
                await orderService.UpdateAsync(order.Id, null, null, orderStateId);
            }
            await goodsCarService.DeleteListAsync(user.Id);
            await orderApplyService.DeleteListAsync(user.Id);
            return new ApiResult { status = 1, msg = "支付成功" };
        }
        [HttpPost]
        public async Task<ApiResult> State()
        {
            IdNameDTO[] res = await idNameService.GetByTypeNameAsync("订单状态");
            //var result = res.Where(i=>i.Name!="退货中").Select(i => new { id = i.Id, name = i.Name }).ToList();
            var result = res.Select(i => new { id = i.Id, name = i.Name }).ToList();
            return new ApiResult { status = 1, data = result };
        }

        [HttpPost]
        public async Task<ApiResult> Discount()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var dto = await userService.GetModelAsync(user.Id);
            decimal discount1 = Convert.ToDecimal(await settingService.GetParmByNameAsync("普通会员优惠"));
            decimal discount2 = Convert.ToDecimal(await settingService.GetParmByNameAsync("黄金会员优惠"));
            decimal discount3 = Convert.ToDecimal(await settingService.GetParmByNameAsync("铂金会员优惠"));

            if (dto.LevelName == "普通会员")
            {
                return new ApiResult { status = 1, data = (discount1 * 10) / 100 };
            }
            else if (dto.LevelName == "黄金会员")
            {
                return new ApiResult { status = 1, data = (discount2 * 10) / 100 };
            }
            else
            {
                return new ApiResult { status = 1, data = (discount3 * 10) / 100 };
            }
        }
    }
}