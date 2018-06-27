using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IUserService:IServiceSupport
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="mobile">账号</param>
        /// <param name="password">密码</param>
        /// <param name="levelTypeId">等级id</param>
        /// <returns></returns>
        Task<long> AddAsync(string mobile, string password, long levelTypeId);
        /// <summary>
        /// 添加推荐人
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="recommendId">推荐人id</param>
        /// <returns></returns>
        Task<long> AddRecommendAsync(long userId, string recommendMobile);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(long id);
        /// <summary>
        /// 冻结用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        Task<bool> FrozenAsync(long id);
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task<bool> ResetPasswordAsync(long id, string password);
        Task<long> UserCheck(string mobile);
        /// <summary>
        /// 根据id获得用户模型
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        Task<UserDTO> GetModelAsync(long id);
        /// <summary>
        /// 根据用户账号获得用户模型
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        Task<UserDTO> GetModelByMobileAsync(string mobile);
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="levelId">用户等级id</param>
        /// <param name="keyword">搜索关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        Task<UserSearchResult> GetModelListAsync(long? levelId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        /// <summary>
        /// 获取一个用户下推荐的团队
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="teamLevel">推荐等级</param>
        /// <param name="keyword">搜索关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        Task<UserTeamSearchResult> GetModelTeamListAsync(long userId, long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
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
