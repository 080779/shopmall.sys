using IMS.Common;
using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Notice
{
    public class ListViewModel
    {
        public NoticeDTO[] Notices { get; set; }
        public PageResult PageResult { get; set; }
    }
}