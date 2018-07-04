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
            dto.SalesAmount = entity.SalesAmount;
            dto.IsReturned = entity.IsReturned;
            dto.IsUpgraded = entity.IsUpgraded;
            return dto;
        }

        public async Task<long> AddAsync(string mobile, string password, long levelTypeId ,string recommendMobile)
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

                long recommendId = (await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == recommendMobile)).Id;
                RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().SingleOrDefaultAsync(u => u.UserId == recommendId);

                if (recommend == null)
                {
                    return -2;
                }
                RecommendEntity ruser = new RecommendEntity();
                ruser.UserId = entity.Id;
                ruser.RecommendId = recommendId;
                ruser.RecommendGenera = recommend.RecommendGenera + 1;
                ruser.RecommendPath = recommend.RecommendPath + "-" + entity.Id;

                dbc.Recommends.Add(ruser);
                await dbc.SaveChangesAsync();
                return user.Id;
            }
        }

        public async Task<bool> UpdateInfoAsync(long id, string nickName, string headpic)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                if(nickName!=null)
                {
                    entity.NickName = nickName;
                }
                if(headpic!=null)
                {
                    entity.HeadPic = headpic;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<long> AddRecommendAsync(long userId, string recommendMobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == userId);
                long recommendId = (await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == recommendMobile)).Id;
                RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().SingleOrDefaultAsync(u => u.UserId == recommendId);
                if (user == null)
                {
                    return -1;
                }
                if(recommend==null)
                {
                    return -2;
                }
                RecommendEntity ruser = new RecommendEntity();
                ruser.UserId = userId;
                ruser.RecommendId = recommendId;
                ruser.RecommendGenera = recommend.RecommendGenera + 1;
                ruser.RecommendPath = recommend.RecommendPath+"-"+userId;

                dbc.Recommends.Add(ruser);
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

        public async Task<long> ResetPasswordAsync(long id, string password, string newPassword)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                if (entity.Password != CommonHelper.GetMD5(password + entity.Salt))
                {
                    return -2;
                }
                entity.Password = CommonHelper.GetMD5(newPassword+entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> UserCheck(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                return entity.Id;
            }
        }

        public async Task<long> CheckLoginAsync(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                if(entity.Password!=CommonHelper.GetMD5(password+entity.Salt))
                {
                    return -2;
                }
                return entity.Id;
            }
        }

        public async Task<long> BalancePayAsync(long id, decimal amount)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u=>u.Id==id);
                if(user==null)
                {
                    return -1;
                }
                if(amount>user.Amount)
                {
                    return -2;
                }
                user.Amount = user.Amount - amount;
                await dbc.SaveChangesAsync();
                return 1;
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

        public async Task<UserSearchResult> GetModelListAsync(long? levelId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var users = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false);

                if(levelId!=null)
                {
                    users = users.Where(a => a.LevelId ==levelId);
                }
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
                result.PageCount = (int)Math.Ceiling((await users.LongCountAsync()) * 1.0f / pageSize);
                var userResult = await users.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Users = userResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
        public async Task<UserTeamSearchResult> GetModelTeamListAsync(long userId, long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserTeamSearchResult result = new UserTeamSearchResult();
                RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().SingleOrDefaultAsync(r => r.UserId == userId);
                var recommends = dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false);
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
                result.PageCount = (int)Math.Ceiling(recommends.LongCount() * 1.0f / pageSize);
                var userResult = await recommends.OrderByDescending(a => a.User.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Users = userResult.Select(a => ToDTO(a.User)).ToArray();
                return result;
            }
        }
    }
}
