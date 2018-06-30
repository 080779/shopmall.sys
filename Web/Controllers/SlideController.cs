﻿using IMS.Common;
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
    public class SlideController : ApiController
    {
        public ISlideService slideService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List()
        {
            SlideSearchResult result = await slideService.GetModelListAsync(null,null,null,1,100);
            List<SlideListApiModel> model;
            model = result.Slides.Select(n => new SlideListApiModel { id = n.Id, name = n.Name,imgUrl=n.ImgUrl, url = n.Url }).ToList(); ;
            return new ApiResult { status = 1, data = model };
        }
    }
}
