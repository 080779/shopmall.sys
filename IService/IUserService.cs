using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IUserService:IServiceSupport
    {
        Task<long> AddAsync(string mobile, string password, long levelTypeId);
        Task<long> AddRecommendAsync(long userId, long recommendId);
        Task<bool> DeleteAsync(long id);
        Task<bool> FrozenAsync(long id);
        Task<bool> ResetPasswordAsync(long id, string password);
        Task<UserDTO> GetModelAsync(long id);
        Task<UserSearchResult> GetModelListAsync(long? levelId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<UserTeamSearchResult> GetModelTeamListAsync(long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class UserSearchResult
    {
        public UserDTO[] Users { get; set; }
        public long TotalCount { get; set; }
    }
    public class UserTeamSearchResult
    {
        public UserDTO TeamUser { get; set; }
        public UserDTO[] Users { get; set; }
        public long TotalCount { get; set; }
    }
}
