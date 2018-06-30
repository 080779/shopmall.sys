using IMS.Service;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 订单实体类
    /// </summary>
    public class OrderEntity : BaseEntity
    {
        public string Code { get; set; }
        public long BuyerId { get; set; }
        public virtual UserEntity Buyer { get; set; }
        public long AddressId { get; set; }
        public virtual AddressEntity Address { get; set; }
        public decimal Amount { get; set; } = 0;
        public decimal PostFee { get; set; } = 0;//邮费
        public long PayTypeId { get; set; }
        public virtual IdNameEntity PayType { get; set; }//支付方式
        public long OrderStateId { get; set; }
        public virtual IdNameEntity OrderState { get; set; }//订单状态
        public DateTime? PayTime { get; set; }//支付时间
        public DateTime? ConsignTime { get; set; }//发货时间
        public DateTime? EndTime { get; set; }//交易完成时间
        public DateTime? CloseTime { get; set; }//交易结束时间
        public virtual DeliveryEntity Delivery { get; set; }
        public string BuyerMessage { get; set; }//买家留言
        public bool IsRated { get; set; } = false;//是否评价
        public DateTime? ApplyTime { get; set; }//申请退货时间
        public decimal? ReturnAmount { get; set; }//申请退货金额
        public decimal? DeductAmount { get; set; } //扣除金额
        public decimal? RefundAmount { get; set; } //应退款金额
        public long? DownCycledId { get; set; }//降级处理
        public long? AuditStatusId { get; set; }//审核状态
        public DateTime? AuditTime { get; set; }//审核时间
        public string AuditMobile { get; set; }//审核人账号
    }
}
