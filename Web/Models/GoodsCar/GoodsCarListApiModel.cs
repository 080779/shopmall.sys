using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.GoodsCar
{
    public class GoodsCarListApiModel
    {
        public long id { get; set; }
        public string name { get; set; }
        public decimal realityPrice { get; set; }//现价
        public long number { get; set; }
    }
}