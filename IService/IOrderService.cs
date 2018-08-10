using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 订单管理接口
    /// </summary>
    public interface IOrderService : IServiceSupport
    {
        Task<long> AddAsync(long buyerId,long addressId,long payTypeId, long orderStateId, long goodsId, long number);
        Task<long> AddAsync(long? deliveryTypeId, decimal? postFee, long buyerId, long addressId, long payTypeId, long orderStateId, params OrderApplyDTO[] orderApplies);
        Task<bool> AddUserDeliverAsync(long orderId,string deliverCode,string deliverName);
        Task<bool> UpdateAsync(long id, long? addressId, long? payTypeId, long? orderStateId);
        Task<long> UpdateDeliverStateAsync(long id, string deliver, string userDeliveryName, string userDeliveryCode);
        Task<bool> Receipt(long id,long orderStateId);
        Task<bool> FrontMarkDel(long id);
        Task<bool> DeleteAsync(long id);
        Task<OrderDTO> GetModelAsync(long id);
        Task<OrderDTO[]> GetAllAsync();
        Task<OrderSearchResult> GetModelListAsync(long? buyerId,long? orderStateId , long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<OrderSearchResult> GetRefundModelListAsync(long? buyerId, long? orderStateId, long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<OrderSearchResult> GetReturnModelListAsync(long? buyerId, long? orderStateId, long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<OrderSearchResult> GetDeliverModelListAsync(long? buyerId, long? orderStateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<long> ApplyReturnOrderAsync(long orderId);
        Task<long> ReturnOrderAsync(long orderId);
        Task<long> ApplyReturnAsync(long orderId);
        Task<long> ReturnAsync(long orderId);
        Task<long> RefundAuditAsync(long orderId, long adminId);
        Task<long> ReturnAuditAsync(long orderId, long adminId);
        Task AutoConfirmAsync();
        void AutoConfirm();
        Task<long> ValidOrder(long id);
    }
    public class OrderSearchResult
    {
        public OrderDTO[] Orders { get; set; }
        public long PageCount { get; set; }
    }    
}
