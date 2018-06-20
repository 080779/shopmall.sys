using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface ITakeCashService:IServiceSupport
    {
        Task<TakeCashSearchResult> GetModelListAsync(long? stateId,string mobile,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
        Task<decimal> CalcAsync(string description,long integral);
    }
    public class TakeCashSearchResult
    {
        public TakeCashDTO[] TakeCashes { get; set; }
        public long TotalCount { get; set; }
        public long? GivingIntegralCount { get; set; }
        public long? UseIntegralCount { get; set; }
    }
}
