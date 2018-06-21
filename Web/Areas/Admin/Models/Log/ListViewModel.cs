using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Log
{
    public class ListViewModel
    {
        public AdminLogDTO[] AdminLogs { get; set; }
        public string PageHtml { get; set; }
        public List<Page> Pages { get; set; }
        public int PageCount { get; set; }
    }
}