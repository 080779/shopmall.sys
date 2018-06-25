﻿using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 公告栏管理接口
    /// </summary>
    public interface INoticeService : IServiceSupport
    {
        Task<long> AddAsync(string content,string url,DateTime failureTime, bool isEnabled);
        Task<bool> UpdateAsync(long id, string content, string url, DateTime failureTime, bool isEnabled);
        Task<bool> DeleteAsync(long id);
        Task<SlideSearchResult> GetModelListAsync(string keyword,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class NoticeSearchResult
    {
        public NoticeDTO[] Notices { get; set; }
        public long TotalCount { get; set; }
    }   
}