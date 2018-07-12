using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderListApiModel
    {
        public List<Order> Orders { get; set; }
        public long PageCount { get; set; }
    }
    public class Order
    {
        public long id { get; set; }
        public string code { get; set; }        
        public decimal amount { get; set; }
        public long payTypeId { get; set; }
        public string payTypeName { get; set; }//支付方式
        public long orderStateId { get; set; }
        public string orderStateName { get; set; }//订单状态
        public string deliveryName { get; set; }//物流名称
        public string deliveryCode { get; set; }//物流单号
        public DateTime createTime { get; set; }
        public List<OrderGoods> OrderGoods { get; set; }
    }
    public class OrderGoods
    {
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal realityPrice { get; set; }
        public decimal totalFee { get; set; }
        public long number { get; set; }
    }
}