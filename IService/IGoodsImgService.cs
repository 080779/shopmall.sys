﻿using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 商品图片管理接口
    /// </summary>
    public interface IGoodsImgService : IServiceSupport
    {
        Task<long> AddAsync(string name,string imgUrl, string description);
        Task<bool> UpdateAsync(long id, string name, string imgUrl, string description);
        Task<bool> DeleteAsync(long id);
        Task<GoodsImgSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class GoodsImgSearchResult
    {
        public GoodsImgDTO[] GoodsImgs { get; set; }
        public long PageCount { get; set; }
    }
}
