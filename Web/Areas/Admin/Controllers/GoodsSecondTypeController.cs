using IMS.Common;
using IMS.DTO;
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
    public class GoodsSecondTypeController : Controller
    {
        private int pageSize = 10;
        public IGoodsSecondTypeService goodsSecondTypeService { get; set; }
        public ActionResult List(long id)
        {
            return View(id);
        }
        [HttpPost]
        public async Task<ActionResult> List(long id,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            GoodsSecondTypeSearchResult result= await goodsSecondTypeService.GetModelListAsync(id,keyword,startTime,endTime,pageIndex,pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }

        public async Task<ActionResult> Add(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商品二级分类名不能为空" });
            }
            long id = await goodsSecondTypeService.AddAsync(name, description);
            if (id <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加商品二级分类失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加商品二级分类成功" });
        }

        public async Task<ActionResult> GetModel(long id)
        {
            GoodsSecondTypeDTO model = await goodsSecondTypeService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        public async Task<ActionResult> Edit(long id, string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商品二级分类名不能为空" });
            }
            bool flag = await goodsSecondTypeService.UpdateAsync(id, name, description);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "编辑商品二级分类失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "编辑商品二级分类成功" });
        }
        public async Task<ActionResult> Del(long id)
        {
            bool flag = await goodsSecondTypeService.DeleteAsync(id);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除商品二级分类失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除商品二级分类成功" });
        }
    }
}