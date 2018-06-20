using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IJournalService:IServiceSupport
    {
        Task<JournalSearchResult> GetModelListAsync(long? id, long? typeId, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<JournalSearchResult> GetIntegralModelListAsync(long? typeId, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<JournalSearchResult> GetGivingModelListAsync(long? id, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<JournalSearchResult> GetSpendModelListAsync(long? id, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<JournalSearchResult> GetModelListAsync(string typeName, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<JournalSearchResult> GetUserModelListAsync(long id, int pageIndex, int pageSize);
        Task<JournalSearchResult> GetMerchantModelListAsync(long? id, long? typeId, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<JournalSearchResult> GetAgencyModelListAsync(long? id,long? typeId,string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class JournalSearchResult
    {
        public JournalDTO[] Journals { get; set; }
        public long TotalCount { get; set; }
        public long PlatformIntegral { get; set; }
        public long? GivingIntegrals { get; set; }
        public long? UseIntegrals { get; set; }
        public long? GivingIntegralCount { get; set; }
        public long? UseIntegralCount { get; set; }
    }
}
