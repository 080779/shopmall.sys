using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Journal
{
    public class ListViewModel
    {
        public JournalDTO[] Journals { get; set; }
        public long PageCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTakeCash { get; set; }
        public decimal TotalBuyAmount { get; set; }
    }
}