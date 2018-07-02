using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Goods;
using IMS.Web.Models.GoodsCar;
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
    public class GoodsCarController : ApiController
    {
        public IGoodsCarService goodsCarService { get; set; }
        [HttpPost]
        public ApiResult List()
        {
            return new ApiResult { status = 1 };
        }        
        [HttpPost]
        public async Task<ApiResult> Add(GoodsCarAddModel model)
        {
            if(model.Id<=0)
            {
                return new ApiResult { status = 0, msg = "商品id错误" };
            }
            if (model.Number <= 0)
            {
                return new ApiResult { status = 0, msg = "商品数量错误" };
            }
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id= await goodsCarService.AddAsync(user.Id, model.Id, model.Number);
            if (id<=0)
            {
                return new ApiResult { status = 0, msg = "添加购物车失败" };
            }
            return new ApiResult { status = 1, msg = "添加购物车成功" };
        }
    }
}