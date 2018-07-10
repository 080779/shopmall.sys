using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Order
{
    public class OrderDetailViewModel
    {
        public BasicInfo BasicInfo { get; set; }
        public BuyerInfo BuyerInfo { get; set; }
        public List<OrderGoodsInfo> OrderGoodsInfos { get; set; }
        public ReturnInfo ReturnInfo { get; set; }
        public List<ReturnGoodsInfo> ReturnGoodsInfos { get; set; }
    }
    public class BasicInfo
    {
        public string Code { get; set; }
        public string OrderStateName { get; set; }
        public Decimal Amount { get; set; }
        public string PayTypeName { get; set; }
        public DateTime CreateTime { get; set; }
    }
    public class BuyerInfo
    {
        public string BuyerMobile { get; set; }
        public string Name { get; set; }//收货人姓名
        public string Mobile { get; set; }//收货人手机号
        public string Address { get; set; }//收货人地址 
    }
    public class OrderGoodsInfo
    {
        public long Id { get; set; }
        public string Thumb { get; set; }//缩略图
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal RealityPrice { get; set; }
        public long Number { get; set; }
    }
    public class ReturnInfo
    {

    }
    public class ReturnGoodsInfo
    {

    }
}