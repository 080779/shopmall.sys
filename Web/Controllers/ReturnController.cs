using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Return;
using IMS.Web.Models.TakeCash;
using IMS.Web.Models.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{    
    public class ReturnController : ApiController
    {        
        public IOrderListService orderListService { get; set; }
        public IOrderService orderService { get; set; }
        [HttpPost]
        public async Task<ApiResult> Select(ReturnSelectModel model)
        {
            bool flag = await orderListService.SetIsReturnAsync(model.Id);
            return new ApiResult { status = 1, msg = "操作成功" }; 
        }
        [HttpPost]
        public async Task<ApiResult> Apply(ReturnApplyModel model)
        {
            long res = await orderService.ApplyReturnAsync(model.OrderId);
            if(res<=0)
            {
                if(res==-4)
                {
                    return new ApiResult { status = 0, msg = "申请退货失败,确认收货三天后不能退货" };
                }
                return new ApiResult { status = 0, msg = "申请退货失败" };
            }
            return new ApiResult { status = 1, msg = "申请退货成功" };
        }

        [HttpPost]
        public async Task<ApiResult> AddDeliver(ReturnAddDeliverrModel model)
        {
            if (string.IsNullOrEmpty(model.DeliverName))
            {
                return new ApiResult { status = 0, msg = "快递名称不能为空" };
            }
            if (string.IsNullOrEmpty(model.DeliverCode))
            {
                return new ApiResult { status = 0, msg = "快递单号不能为空" };
            }
            bool flag = await orderService.AddUserDeliverAsync(model.OrderId, model.DeliverCode, model.DeliverName);
            if(!flag)
            {
                return new ApiResult { status = 0, msg = "添加快递单号失败" };
            }
            return new ApiResult { status = 1, msg = "添加快递单号成功" };
        }
    }    
}