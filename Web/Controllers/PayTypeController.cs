using IMS.Common;
using IMS.IService;
using IMS.Web.Models.Notice;
using IMS.Web.Models.Slide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{   
    public class PayTypeController : ApiController
    {
        public IIdNameService idNameService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List()
        {
            var res= await idNameService.GetByTypeNameAsync("支付方式");
            List<PayTypeListApiModel> model;
            model = res.Select(n => new PayTypeListApiModel { id = n.Id, name = n.Name}).ToList(); 
            return new ApiResult { status = 1, data = model };
        }
    }
}
