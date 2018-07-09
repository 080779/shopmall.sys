using IMS.Common;
using IMS.DTO;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class NoticeController : Controller
    {
        private int pageSize = 10;
        public INoticeService noticeService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> List(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await noticeService.GetModelListAsync(mobile, startTime, endTime, pageIndex, pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        public async Task<ActionResult> Add(string content, string url, DateTime faliureTime)
        {
            if (string.IsNullOrEmpty(content))
            {
                return Json(new AjaxResult { Status = 0, Msg = "幻灯片名称不能为空" });
            }
            if (string.IsNullOrEmpty(url))
            {
                return Json(new AjaxResult { Status = 0, Msg = "转向连接不能为空" });
            }
            long id = await noticeService.AddAsync(content, url, faliureTime);
            if (id <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加公告失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加公告成功" });
        }

        public async Task<ActionResult> GetModel(long id)
        {
            NoticeDTO model = await noticeService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        public async Task<ActionResult> Edit(long id, string content, string url, DateTime faliureTime)
        {
            if (string.IsNullOrEmpty(content))
            {
                return Json(new AjaxResult { Status = 0, Msg = "幻灯片名称不能为空" });
            }
            if (string.IsNullOrEmpty(url))
            {
                return Json(new AjaxResult { Status = 0, Msg = "转向连接不能为空" });
            }
            bool flag = await noticeService.UpdateAsync(id, content, url, faliureTime);

            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改幻灯片失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改公告成功" });
        }
        public async Task<ActionResult> Del(long id)
        {
            bool flag = await noticeService.DeleteAsync(id);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除公告失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除公告成功" });
        }
    }
}