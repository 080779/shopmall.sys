using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Merchant
{
    public class ListViewModel
    {
        public PlatformUserDTO[] PlatformUsers { get; set; }
        public string PageHtml { get; set; }
        public int PageCount { get; set; }
        public List<Page> Pages { get; set; }
        public long PlatformIntegral { get; set; }
    }
}