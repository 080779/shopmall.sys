using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    public class GoodsController : ApiController
    {
        public IGoodsCarService goodsCarService { get; set; }
        [HttpPost]
        public ApiResult List()
        {
            return new ApiResult { status = 1 };
        }
        [HttpPost]
        public async Task<ApiResult> AddCar()
        {
            
            //goodsCarService.AddAsync(1, 1, 1);
            await goodsCarService.UpdateAsync(2,3);
            return new ApiResult { status = 1, msg="ok" };
        }
        [HttpPost]
        public async Task<ApiResult> GetCars()
        {
            List<long> lists=new List<long>();
            //goodsCarService.AddAsync(1, 1, 1);
            GoodsCarSearchResult result= await goodsCarService.GetModelListAsync(1,null,null,null,1,2);
            var datasoure = result.GoodsCars.Select(g => new aa { id=g.Id}).ToList();
            result.GoodsCars.Where(g => true);
            return new ApiResult { status=1,data=result};
        }
        public class aa
        {
            public long id { get; set; }
        }
    }
}