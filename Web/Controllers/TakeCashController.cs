using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
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
    public class TakeCashController : ApiController
    {
        public ITakeCashService takeCashService { get; set; }
        public IIdNameService idNameService { get; set; }
        public async Task<ApiResult> List()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var res = await takeCashService.GetModelListAsync(user.Id, null, null, null, null, 1, 100);
            return new ApiResult { status = 1, data = res };
        }
        public async Task<ApiResult> Apply(TakeCashApplyModel model)
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id = await takeCashService.AddAsync(user.Id, model.PayTypeId, model.Amount, "佣金提现");
            if(id<=0)
            {
                if(id==-1)
                {
                    return new ApiResult { status = 0, msg = "用户不存在" };
                }
                if (id == -2)
                {
                    return new ApiResult { status = 0, msg = "用户账户余额不足" };
                }
                return new ApiResult { status = 0, msg="申请提现失败" };
            }
            return new ApiResult { status = 1, msg = "申请提现成功" };
        }
        public async Task<ApiResult> PayTypes()
        {
            var result = await idNameService.GetByTypeNameAsync("收款方式");
            var res = result.Select(i => new { id = i.Id, name = i.Name });
            return new ApiResult { status = 1, data = res };
        }
    }    
}