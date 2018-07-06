﻿using IMS.DTO;
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
        Task<bool> UpdateAsync(long id, long? addressId, long? payTypeId, long? orderStateId);
        Task<bool> DeleteAsync(long id);
        Task<OrderDTO> GetModelAsync(long id);
        Task<OrderSearchResult> GetModelListAsync(long? buyerId,long? orderStateId ,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class OrderSearchResult
    {
        public OrderDTO[] Orders { get; set; }
        public long PageCount { get; set; }
    }    
}