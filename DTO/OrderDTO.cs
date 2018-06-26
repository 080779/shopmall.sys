using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class OrderDTO : BaseDTO
    {
        public string Code { get; set; }
        public long BuyerId { get; set; }
        public string BuyerNickName { get; set; }
        public decimal Amount { get; set; }
        public decimal PostFee { get; set; }//邮费
        public long PayTypeId { get; set; }
        public string PayTypeName { get; set; }//支付方式
        public long OrderStateId { get; set; }
        public string OrderStateName { get; set; }//订单状态
        public string DeliveryName { get; set; }//物流名称
        public string DeliveryCode { get; set; }//物流单号
        public string ReceiverName { get; set; }
        public string ReceiverMobile { get; set; }
        public string ReceiverAddress { get; set; }
        public DateTime? PayTime { get; set; }//支付时间
        public DateTime? ConsignTime { get; set; }//发货时间
        public DateTime? EndTime { get; set; }//交易完成时间
        public DateTime? CloseTime { get; set; }//交易结束时间
        public string BuyerMessage { get; set; }//买家留言
        public bool IsRated { get; set; } //是否评价
        public DateTime? ApplyTime { get; set; }//申请退货时间
        public decimal? DeductAmount { get; set; } //扣除金额
        public decimal? RefundAmount { get; set; } //应退款金额
    }
}
