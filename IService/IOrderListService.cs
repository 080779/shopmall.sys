﻿using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 订单列表管理接口
    /// </summary>
    public interface IOrderListService : IServiceSupport
    {
        Task<long> AddAsync(long orderId, long goodsId, long number);
        Task<long> AddListAsync(List<OrderListAdd> goodsLists);
        Task<bool> UpdateAsync(long id, long number);
        Task<bool> DeleteAsync(long id);
        Task<OrderListSearchResult> GetModelListAsync(long? orderId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class OrderListSearchResult
    {
        public OrderListDTO[] OrderLists { get; set; }
        public long PageCount { get; set; }
    }
    public class OrderListAdd
    {
        public long OrderId { get; set; }
        public long GoodsId { get; set; }
        public long Number { get; set; }
    }
}
