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
        public IGoodsSecondTypeService goodsSecondTypeService { get; set; }
        public IGoodsImgService goodsImgService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(long? goodsTypeId, long? goodsSecondTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await goodsService.GetModelListAsync(null, goodsTypeId, goodsSecondTypeId, keyword, startTime, endTime, pageIndex, pageSize);
            GoodsListViewModel model = new GoodsListViewModel();
            model.Goods = result.Goods;
            model.PageCount = result.PageCount;
            model.GoodsTypes = (await goodsTypeService.GetModelListAsync(null, null, null, 1, 100)).GoodsTypes;
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        [HttpPost]
        public async Task<ActionResult> Upload(long id, string[] imgFiles)
        {
            if (imgFiles.Count() <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "请选择上传的图片" });
            }
            List<string> lists = new List<string>();
            foreach (string imgFile in imgFiles)
            {
                if (imgFile.Contains("upload/"))
                {
                    lists.Add(imgFile);
                }
                else
                {
                    string res;
                    if (!ImageHelper.SaveBase64(imgFile, out res))
                    {
                        return Json(new AjaxResult { Status = 0, Msg = res });
                    }
                    lists.Add(res);
                }
            }
            List<string> imgUrls = lists.Distinct().ToList();
            long resid = await goodsImgService.AddAsync(id, imgUrls);
            if (resid <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "上传失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "上传成功" });
        }

        [HttpPost]
        public async Task<ActionResult> GetImg(long id)
        {
            var res = await goodsImgService.GetModelListAsync(id);
            string imgUrl;
            List<string> lists = res.Select(g => imgUrl = g.ImgUrl).ToList();
            return Json(new AjaxResult { Status = 1, Data = lists });
        }
        [HttpPost]
        public async Task<ActionResult> GetSecondType(long id)
        {
            var res = await goodsSecondTypeService.GetModelListAsync(id, null, null, null, 1, 100);
            var result = res.GoodsSecondTypes.Select(g => new { id = g.Id, name = g.Name }).ToList();
            return Json(new AjaxResult { Status = 1, Data = result });
        }

        [HttpPost]
        public async Task<ActionResult> Add(GoodsAddEditModel model)
        {
            long id = await goodsService.AddAsync(model);
            if(id<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "商品添加失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "商品添加成功" });
        }
    }
}