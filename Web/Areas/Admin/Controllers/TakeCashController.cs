﻿using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.TakeCash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class TakeCashController : Controller
    {
        public ITakeCashService takeCashService { get; set; }
        public IIdNameService idNameService { get; set; }
        private int pageSize = 10;
        //[Permission("积分管理_积分管理")]
        public ActionResult List()
        {
            return View();
        }
        //[Permission("积分管理_积分管理")]
        [HttpPost]
        public async Task<ActionResult> List(long? stateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            TakeCashSearchResult result = await takeCashService.GetModelListAsync(null,stateId, keyword, startTime, endTime, pageIndex, pageSize);
            TakeCashListViewModel model = new TakeCashListViewModel();
            model.TakeCashes = result.TakeCashes;
            model.PageCount = result.PageCount;
            model.States = await idNameService.GetByTypeNameAsync("提现状态");
            return Json(new AjaxResult { Status = 1, Data = model });
        }        
    }
}