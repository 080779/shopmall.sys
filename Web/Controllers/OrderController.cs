using IMS.Common;
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
        [HttpPost]
        public async Task<ApiResult> Add(OrderAddModel model)
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id= await orderService.AddAsync(user.Id, model.AddressId, model.PayTypeId, model.OrderStateId,model.GoodsId,model.Number);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "生成订单失败" };
            }
            return new ApiResult { status = 1,msg= "生成订单成功" };
        }

        [HttpPost]
        public async Task<ApiResult> Pay(OrderAddModel model)
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id = await orderService.AddAsync(user.Id, model.AddressId, model.PayTypeId, model.OrderStateId, model.GoodsId, model.Number);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "生成订单失败" };
            }
            long payTypeId= await idNameService.GetIdByNameAsync("余额");
            OrderDTO dto = await orderService.GetModelAsync(id);
            if(payTypeId==model.PayTypeId)
            {
                long payResId= await userService.BalancePayAsync(user.Id, dto.Amount);
                if(payResId==-1)
                {
                    return new ApiResult { status = 0, msg = "用户不存在" };
                }
                if (payResId == -2)
                {
                    return new ApiResult { status = 0, msg = "用户账户余额不足" };
                }
            }
            return new ApiResult { status = 1, msg = "生成订单成功" };
        }
    }
}