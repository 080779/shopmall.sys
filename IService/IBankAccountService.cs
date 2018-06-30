using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 银行卡管理接口
    /// </summary>
    public interface IBankAccountService : IServiceSupport
    {
        Task<long> AddAsync(long userId,string name, string account,string accountName);
        Task<bool> UpdateAsync(long id, string name, string account, string accountName);
        Task<bool> DeleteAsync(long id);
        Task<BankAccountDTO[]> GetModelByUserIdAsync(long id);
        Task<BankAccountSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class BankAccountSearchResult
    {
        public BankAccountDTO[] BankAccounts { get; set; }
        public long PageCount { get; set; }
    }
}
