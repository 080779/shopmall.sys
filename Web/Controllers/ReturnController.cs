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
        [HttpPost]
        public async Task<ApiResult> Select(ReturnSelectModel model)
        {
            bool flag = await orderListService.SetIsReturnAsync(model.Id);
            return new ApiResult { status = 1, msg = "操作成功" };
        }
        [HttpPost]
        public async Task<ApiResult> Apply(ReturnApplyModel model)
        {
            bool flag = await orderListService.SetIsReturnAsync(model.OrderId);
            return new ApiResult { status = 1, msg = "操作成功" };
        }
    }    
}