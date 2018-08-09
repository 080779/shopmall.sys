using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.GoodsType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    public class GoodsTypeController : ApiController
    {
        public IGoodsTypeService goodsTypeService { get; set; }
        public IGoodsSecondTypeService goodsSecondTypeService { get; set; }
        public IMainGoodsTypeService mainGoodsTypeService { get; set; }
        public ISettingService settingService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List()
        {
            string parm = await settingService.GetParmByNameAsync("网站域名");
            GoodsTypeSearchResult result= await goodsTypeService.GetModelListAsync(null, null, null, 1, 100);
            List<GoodsTypeListApiModel> model;
            model = result.GoodsTypes.Select(g => new GoodsTypeListApiModel { id = g.Id, name = g.Name, imgUrl = parm + g.ImgUrl }).ToList();
            return new ApiResult { status = 1, data = model };
        }
        [HttpPost]
        public async Task<ApiResult> SecondList(GoodsTypeSecondListModel model)
        {
            GoodsSecondTypeSearchResult result = await goodsSecondTypeService.GetModelListAsync(model.Id,null, null, null, 1, 100);
            List<GoodsTypeListApiModel> listModel;
            listModel = result.GoodsSecondTypes.Select(g => new GoodsTypeListApiModel { id = g.Id, name = g.Name }).ToList();
            return new ApiResult { status = 1, data = listModel };
        }
        [HttpPost]
        public async Task<ApiResult> MainList()
        {
            MainGoodsTypeSearchResult result = await mainGoodsTypeService.GetModelListAsync(null, null, null, 1, 100);
            List<GoodsTypeMainListApiModel> model;
            model = result.MainGoodsTypes.Select(g => new GoodsTypeMainListApiModel { id = g.Id, name = g.Name }).ToList();
            return new ApiResult { status = 1, data = model };
        }
    }
}