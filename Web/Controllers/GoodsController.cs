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
        private string TokenSecret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
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

            var user= JwtHelper.JwtDecrypt<User>(ControllerContext, TokenSecret);
            //User user = new User();
            //user.UserId = 1;
            //user.UserName = "vz";
            //string token=JwtHelper.JwtEncrypt(ControllerContext, user, TokenSecret);
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