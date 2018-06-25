using IMS.Common;
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
            return dto;
        }

        public async Task<long> AddAsync(string mobile, string password, long levelTypeId)
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
                user.Salt = CommonHelper.GetCaptcha(4);
                user.Password = CommonHelper.GetMD5(password + user.Salt);
                dbc.Users.Add(user);
                await dbc.SaveChangesAsync();
                return user.Id;
            }
        }
        public async Task<long> AddRecommendAsync(long userId, long recommendId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                RecommendEntity user = await dbc.GetAll<RecommendEntity>().SingleOrDefaultAsync(u => u.UserId == userId);
                RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().SingleOrDefaultAsync(u => u.UserId == recommendId);
                if (user != null)
                {
                    return -1;
                }
                if(recommend==null)
                {
                    return -2;
                }
                user = new RecommendEntity();
                user.UserId = userId;
                user.RecommendId = recommendId;
                user.RecommendGenera = recommend.RecommendGenera + 1;
                user.RecommendPath = recommend.RecommendPath+"-"+userId;

                dbc.Recommends.Add(user);
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
                AddressEntity address = await dbc.GetAll<AddressEntity>().SingleOrDefaultAsync(a=>a.UserId==id);
                if(address!=null)
                {
                    address.IsDeleted = true;
                }
                BankAccountEntity bankAccount = await dbc.GetAll<BankAccountEntity>().SingleOrDefaultAsync(a => a.UserId == id);
                if (bankAccount != null)
                {
                    bankAccount.IsDeleted = true;
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

        public async Task<UserDTO> GetModelByMobileAsync(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u=>u.Mobile==mobile);
                if(user==null)
                {
                    return null;
                }
                return ToDTO(user);
            }
        }

        public async Task<UserSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var users = dbc.GetAll<UserEntity>();

                if (!string.IsNullOrEmpty(keyword))
                {
                    users = users.Where(a => a.Mobile.Contains(keyword) || a.Code.Contains(keyword) || a.NickName.Contains(keyword));
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
        public async Task<UserTeamSearchResult> GetModelTeamListAsync(long? teamLevel,long userId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserTeamSearchResult result = new UserTeamSearchResult();
                RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().SingleOrDefaultAsync(r => r.UserId == userId);
                var recommends = dbc.GetAll<RecommendEntity>();
                if (teamLevel!=null)
                {
                    if(teamLevel==1)
                    {
                        recommends = recommends.Where(a => a.UserId== userId);
                    }
                    else if(teamLevel==2)
                    {
                        recommends = recommends.Where(a => a.RecommendPath.Contains("-"+ userId.ToString()+"-") && a.RecommendGenera==recommend.RecommendGenera+2);
                    }
                    else if (teamLevel == 3)
                    {
                        recommends = recommends.Where(a => a.RecommendPath.Contains("-" + userId.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 3);
                    }
                }
                if(keyword!=null)
                {
                    recommends = recommends.Where(a => a.User.Mobile.Contains(keyword) || a.User.Code.Contains(keyword) || a.User.NickName.Contains(keyword));
                }
                if (startTime != null)
                {
                    recommends = recommends.Where(a => a.User.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    recommends = recommends.Where(a => a.User.CreateTime.Year <= endTime.Value.Year && a.User.CreateTime.Month <= endTime.Value.Month && a.User.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = recommends.LongCount();
                var userResult = await recommends.OrderByDescending(a => a.User.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Users = userResult.Select(a => ToDTO(a.User)).ToArray();
                return result;
            }
        }
    }
}
