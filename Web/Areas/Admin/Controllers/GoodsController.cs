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
        public IGoodsImgService goodsImgService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(long? goodsTypeId, long? goodsSecondTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await goodsService.GetModelListAsync(null,goodsTypeId,goodsSecondTypeId,keyword, startTime, endTime, pageIndex, pageSize);
            GoodsListViewModel model = new GoodsListViewModel();
            model.Goods = result.Goods;
            model.PageCount = result.PageCount;
            model.GoodsTypes = (await goodsTypeService.GetModelListAsync(null,null,null,1,100)).GoodsTypes;
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        [HttpPost]
        public async Task<ActionResult> Upload(long id,string imgFile)
        {
            if (string.IsNullOrEmpty(imgFile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商品图片必须上传" });
            }
            string res;
            if (!ImageHelper.SaveBase64(imgFile, out res))
            {
                return Json(new AjaxResult { Status = 0, Msg = res });
            }
            long resid= await goodsImgService.AddAsync(id,"", res, "");
            if(resid<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "上传失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg="上传成功" });
        }
    }
}