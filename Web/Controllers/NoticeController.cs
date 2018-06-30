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
        [HttpPost]
        public async Task<ApiResult> List()
        {
            NoticeSearchResult result= await noticeService.GetModelListAsync(null,null,null,1,100);
            List<NoticeListApiModel> model;
            model = result.Notices.Select(n => new NoticeListApiModel { id = n.Id, content = n.Content, url = n.Url }).ToList(); ;
            return new ApiResult { status = 1, data = model };
        }
    }
}
