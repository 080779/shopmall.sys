using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderReApplysModel
    {
        public long OrderId { get; set; }
        public long AddressId { get; set; }
        public long PayTypeId { get; set; }
    }
}