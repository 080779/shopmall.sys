using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 设置管理接口
    /// </summary>
    public interface ISettingService : IServiceSupport
    {
        Task<long> AddAsync(string name, long sttingTypeId, string description);
        Task<bool> UpdateAsync(long id, string parm);
        Task<bool> UpdateAsync(params SettingParm[] parms);
        Task<bool> DeleteAsync(long id);
        Task<SettingDTO> GetModelAsync(long id);
        Task<SettingDTO> GetModelByNameAsync(string name);
        Task<string> GetParmByNameAsync(string name);
        Task<SettingDTO[]> GetModelListAsync(string settingTypeName);
        Task<SettingDTO[]> GetModelListAsync(long[] settingTypeIds);
        Task<SettingSearchResult> GetModelListAsync(long[] settingTypeIds, string keyword,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class SettingSearchResult
    {
        public SettingDTO[] Settings { get; set; }
        public long PageCount { get; set; }
    }
    public class SettingParm
    {
        public long Id { get; set; }
        public string Parm { get; set; }
    }
}
