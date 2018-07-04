using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 首页商品分类管理接口
    /// </summary>
    public interface IMainGoodsTypeService : IServiceSupport
    {
        Task<long> AddAsync(string name, string imgUrl, string description);
        Task<bool> UpdateAsync(long id, string name, string imgUrl, string description);
        Task<bool> DeleteAsync(long id);
        Task<MainGoodsTypeSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class MainGoodsTypeSearchResult
    {
        public MainGoodsTypeDTO[] MainGoodsTypes { get; set; }
        public long PageCount { get; set; }
    }   
}
