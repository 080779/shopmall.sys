﻿using IMS.DTO;
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
        Task<long> AddAsync(string mobile, string password, long levelTypeId, string recommendMobile);
        /// <summary>
        /// 修改头像，昵称
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="nickName">要修改的昵称（为null不修改）</param>
        /// <param name="headpic">要修改的头像地址（为null不修改）</param>
        /// <returns></returns>
        Task<bool> UpdateInfoAsync(long id,string nickName, string headpic);
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
        Task<long> ResetPasswordAsync(long id, string password, string newPassword);
        Task<long> ResetPasswordAsync(long id, string password);
        Task<long> UserCheck(string mobile);
        Task<long> CheckLoginAsync(string mobile, string password);
        Task<long> BalancePayAsync(long id, decimal amount);
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
        Task<UserTeamSearchResult> GetModelTeamListAsync(string mobile, long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class UserSearchResult
    {
        public UserDTO[] Users { get; set; }
        public long PageCount { get; set; }
    }
    public class UserTeamSearchResult
    {
        public UserDTO TeamLeader { get; set; }
        public UserDTO[] Members { get; set; }
        public long PageCount { get; set; }
    }
}
