﻿using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class UserService : IUserService
    {
        public UserDTO ToDTO(UserEntity entity)
        {
            UserDTO dto = new UserDTO();
            dto.Amount = entity.Amount;
            dto.BankAccountId = entity.BankAccountId;
            dto.Code = entity.Code;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.ErrorCount = entity.ErrorCount;
            dto.ErrorTime = entity.ErrorTime;
            dto.Id = entity.Id;
            dto.IsEnabled = entity.IsEnabled;
            dto.LevelId = entity.LevelId;
            dto.LevelName = entity.Level.Name;
            dto.Mobile = entity.Mobile;
            dto.NickName = entity.NickName;
            dto.RecommendCode = entity.Recommend.Code;
            dto.RecommendId = entity.RecommendId;
            dto.RecommendMobile = entity.Recommend.Mobile;
            return dto;
        }

        public async Task<long> AddAsync(string mobile, string password, long levelTypeId,long recommendId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity= await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if(entity!=null)
                {
                    return -1;
                }
                UserEntity user = new UserEntity();
                user.LevelId = levelTypeId;
                user.Mobile = mobile;
                user.RecommendId = recommendId;
                user.Salt = CommonHelper.GetCaptcha(4);
                user.Password = CommonHelper.GetMD5(password + user.Salt);
                dbc.Users.Add(user);
                await dbc.SaveChangesAsync();
                return user.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> FrozenAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsEnabled = !entity.IsEnabled;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> ResetPasswordAsync(long id, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Password = CommonHelper.GetMD5(password+entity.Salt);
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<UserDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<UserSearchResult> GetModelListAsync(string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var users = dbc.GetAll<UserEntity>();
                if (!string.IsNullOrEmpty(mobile))
                {
                    users = users.Where(a => a.Mobile.Contains(mobile));
                }
                if (!string.IsNullOrEmpty(code))
                {
                    users = users.Where(a => a.Code.Contains(code));
                }
                if (startTime != null)
                {
                    users = users.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    users = users.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = users.LongCount();
                var userResult = await users.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Users = userResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}