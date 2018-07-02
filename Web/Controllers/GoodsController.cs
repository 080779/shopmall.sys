using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Goods;
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
    public class GoodsController : ApiController
    {
        public IGoodsService goodsService { get; set; }
        public IGoodsTypeService goodsTypeService { get; set; }
        public IGoodsImgService goodsImgService { get; set; }
        [HttpPost]
        public ApiResult List()
        {
            return new ApiResult { status = 1 };
        }
        [HttpPost]
        public async Task<ApiResult> Search(GoodsSearchModel model)
        {
            GoodsSearchResult result= await goodsService.SearchAsync(model.KeyWord, null, null, model.PageIndex, model.PageSize);
            List<SearchResultModel> lists;
            lists = result.Goods.Select(g => new SearchResultModel { id = g.Id, name = g.Name, realityPrice = g.RealityPrice, saleNum = g.SaleNum }).ToList();
            GoodsSearchApiModel apiModel = new GoodsSearchApiModel();
            apiModel.goods = lists;
            apiModel.pageCount = result.PageCount;
            return new ApiResult { status = 1,data=apiModel };
        }
        [HttpPost]
        public async Task<ApiResult> Detail(GoodsDetailModel model)
        {
            GoodsDTO dto= await goodsService.GetModelAsync(model.Id);
            GoodsImgSearchResult result = await goodsImgService.GetModelListAsync(null, null, null, 1, 100);
            GoodsDetailApiModel apiModel = new GoodsDetailApiModel();
            apiModel.goodsImgs = result.GoodsImgs.Select(g => new GoodsImg { id = g.Id, imgUrl = g.ImgUrl }).ToList();
            if (dto!=null)
            {
                apiModel.id = dto.Id;
                apiModel.description = dto.Description;
                apiModel.inventory = dto.Inventory;
                apiModel.name = dto.Name;
                apiModel.price = dto.Price;
                apiModel.realityPrice = dto.RealityPrice;
                apiModel.saleNum = dto.SaleNum;
            }
            return new ApiResult { status = 1, data = apiModel };
        }
    }
}