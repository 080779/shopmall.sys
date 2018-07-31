using IMS.Common;
using IMS.IService;
using IMS.Web.Models.Notice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{   
    public class NoticeController : ApiController
    {
        public INoticeService noticeService { get; set; }
        public IOrderService orderService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List()
        {
            NoticeSearchResult result= await noticeService.GetModelListAsync(null,null,null,1,100);
            List<NoticeListApiModel> model;
            model = result.Notices.Where(n=>n.IsEnabled==true).Select(n => new NoticeListApiModel { id = n.Id, content = n.Content, code = n.Code }).ToList();
            await orderService.AutoConfirm();
            return new ApiResult { status = 1, data = model };
        }
        public async Task<ApiResult> Detail(NoticeDetailModel model)
        {
            var n= await noticeService.GetModelAsync(model.Id);
            if(n==null)
            {
                return new ApiResult { status = 0, msg = "公告不存在" };
            }
            NoticeListApiModel res = new NoticeListApiModel { id = n.Id, content = n.Content, code = n.Code };
            return new ApiResult { status = 1, data = res };
        }
    }
}
