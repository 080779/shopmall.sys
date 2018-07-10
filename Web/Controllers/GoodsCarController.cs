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
    public class GoodsCarController : ApiController
    {
        public IGoodsCarService goodsCarService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            GoodsCarSearchResult result= await goodsCarService.GetModelListAsync(user.Id, null, null, null, 1, 100);
            List<GoodsCarListApiModel> lists=result.GoodsCars.Select(g => new GoodsCarListApiModel { id = g.Id, name = g.Name, realityPrice = g.RealityPrice, number = g.Number }).ToList();
            return new ApiResult { status = 1, data = lists };
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