using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 商品分类管理接口
    /// </summary>
    public interface IGoodsCarService : IServiceSupport
    {
        Task<long> AddAsync(long userId,long goodsId,long num);
        Task<bool> UpdateAsync(long id,long num);
        Task<bool> DeleteAsync(long id);
        Task<GoodsCarSearchResult> GetModelListAsync(long? userId,string keyword,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class GoodsCarSearchResult
    {
        public GoodsCarDTO[] GoodsCars { get; set; }
        public long TotalCount { get; set; }
    }
    public class Goods
    {
        public long GoodsId { get; set; }
        public long Number { get; set; }
    }
}
