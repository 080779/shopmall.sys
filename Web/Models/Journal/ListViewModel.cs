using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Models.Journal
{
    public class ListViewModel
    {
        public JournalDTO[] Journals { get; set; }
        public JournalTypeDTO[] JournalTypes { get; set; }
        public string PageHtml { get; set; }
        public List<Page> Pages { get; set; }
        public long PageCount { get; set; }
        public long GivingIntegral { get; set; }
        public long UseIntegral { get; set; }
    }
}