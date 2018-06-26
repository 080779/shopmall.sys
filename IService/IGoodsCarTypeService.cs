using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 商品分类管理接口
    /// </summary>
    public interface IGoodsCarTypeService : IServiceSupport
    {
        Task<long> AddAsync(long[] goodsIds);
        Task<bool> UpdateAsync(long id,long[] goodsIds);
        Task<bool> DeleteAsync(long id);
        Task<GoodsCarSearchResult> GetModelListAsync(string keyword,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class GoodsCarSearchResult
    {
        public GoodsCarDTO[] Goods { get; set; }
        public long TotalCount { get; set; }
    }       
}
