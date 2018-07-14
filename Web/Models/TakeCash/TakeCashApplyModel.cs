using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.TakeCash
{
    public class TakeCashApplyModel
    {
        public long PayTypeId { get; set; }
        public decimal Amount { get; set; }
    }
}