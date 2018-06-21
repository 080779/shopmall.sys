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
        public UserEntity Buyer { get; set; }
        public decimal Amount { get; set; }
        public long AddressId { get; set; }
        public AddressEntity Address { get; set; }//收货地址
        public long PayTypeId { get; set; }
        public PayCodeEntity PayType { get; set; }//支付方式
        public long OrderStateId { get; set; }
        public OrderStateEntity OrderState { get; set; }//订单状态
        public DateTime ApplyTime { get; set; }//申请退货时间
        public decimal DeductAmount { get; set; }//扣除金额
        public decimal RefundAmount { get; set; }//应退款金额
    }
}
