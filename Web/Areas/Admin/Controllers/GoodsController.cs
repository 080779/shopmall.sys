using IMS.Common;
using IMS.IService;
using IMS.Web.Areas.Admin.Models.Goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class GoodsController : Controller
    {
        private int pageSize = 10;
        public IGoodsService goodsService { get; set; }
        public IGoodsTypeService goodsTypeService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(long? goodsTypeId, long? goodsSecondTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await goodsService.GetModelListAsync(goodsTypeId,goodsSecondTypeId,keyword, startTime, endTime, pageIndex, pageSize);
            GoodsListViewModel model = new GoodsListViewModel();
            model.Goods = result.Goods;
            model.PageCount = result.PageCount;
            model.GoodsTypes = (await goodsTypeService.GetModelListAsync(null,null,null,1,100)).GoodsTypes;
            return Json(new AjaxResult { Status = 1, Data = model });
        }
    }
}