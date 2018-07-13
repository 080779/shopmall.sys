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
            dto.BuyAmount = entity.BuyAmount;
            dto.IsReturned = entity.IsReturned;
            dto.IsUpgraded = entity.IsUpgraded;
            dto.BonusAmount = entity.BonusAmount;
            dto.Recommonder = entity.Recommend.RecommendMobile;
            return dto;
        }

        public async Task<long> AddAsync(string mobile, string password, long levelTypeId ,string recommendMobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                using (var scope = dbc.Database.BeginTransaction())
                {
                    try
                    {
                        UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                        if (entity != null)
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
                        ruser.UserId = user.Id;
                        ruser.RecommendId = recommendId;
                        ruser.RecommendGenera = recommend.RecommendGenera + 1;
                        ruser.RecommendPath = recommend.RecommendPath + "-" + user.Id;
                        ruser.RecommendMobile = recommend.User.Mobile;

                        dbc.Recommends.Add(ruser);
                        await dbc.SaveChangesAsync();
                        scope.Commit();
                        return user.Id;
                    }
                    catch(Exception ex)
                    {
                        scope.Rollback();
                        throw ex;
                    }
                }                
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
                var address = dbc.GetAll<AddressEntity>().Where(a=>a.UserId==id);
                if (address.LongCount() > 0)
                {
                    await address.ForEachAsync(a => a.IsDeleted = true);
                }
                var bankAccounts = dbc.GetAll<BankAccountEntity>().Where(a => a.UserId == id);
                if (bankAccounts.LongCount()>0)
                {
                    await bankAccounts.ForEachAsync(a => a.IsDeleted = true);
                }
                RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().SingleOrDefaultAsync(r=>r.UserId==id);
                if(recommend!=null)
                {
                    recommend.IsDeleted = true;
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

        public async Task<long> ResetPasswordAsync(long id, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
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
                user.BuyAmount = user.BuyAmount + amount;

                JournalEntity journal = new JournalEntity();
                journal.UserId = user.Id;
                journal.BalanceAmount = user.Amount;
                journal.OutAmount = amount;
                journal.Remark = "购买商品";
                journal.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "购物")).Id;
                dbc.Journals.Add(journal);
                decimal one;
                decimal two;
                decimal three;
                if (user.Level.Name=="普通会员")
                {
                    one = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == user.Level.Name + "一级分销佣金比例")).Parm) / 100;
                    two = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == user.Level.Name + "二级分销佣金比例")).Parm) / 100;
                    three = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == user.Level.Name + "三级分销佣金比例")).Parm) / 100;
                }
                else if(user.Level.Name == "黄金会员")
                {
                    one = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == user.Level.Name + "一级分销佣金比例")).Parm) / 100;
                    two = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == user.Level.Name + "二级分销佣金比例")).Parm) / 100;
                    three = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == user.Level.Name + "三级分销佣金比例")).Parm) / 100;
                }
                else
                {
                    one = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == user.Level.Name + "一级分销佣金比例")).Parm) / 100;
                    two = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == user.Level.Name + "二级分销佣金比例")).Parm) / 100;
                    three = Convert.ToDecimal((await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == user.Level.Name + "三级分销佣金比例")).Parm) / 100;
                }
                UserEntity oneer = dbc.GetAll<UserEntity>().SingleOrDefault(u => u.Id == user.Recommend.RecommendId);
                if (oneer.Recommend.RecommendPath!= "0")
                {
                    oneer.Amount = oneer.Amount + amount * one;
                    oneer.BonusAmount = oneer.BonusAmount + amount * one;

                    JournalEntity journal1 = new JournalEntity();
                    journal1.UserId = oneer.Id;
                    journal1.BalanceAmount = oneer.Amount;
                    journal1.InAmount = amount * one;
                    journal1.Remark = "佣金收入";
                    journal1.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "佣金收入")).Id;
                    dbc.Journals.Add(journal1);

                    UserEntity twoer= dbc.GetAll<UserEntity>().SingleOrDefault(u => u.Id == oneer.Recommend.RecommendId);
                    if(twoer.Recommend.RecommendPath != "0")
                    {
                        twoer.Amount = twoer.Amount + amount * two;
                        twoer.BonusAmount = twoer.BonusAmount + amount * two;

                        JournalEntity journal2 = new JournalEntity();
                        journal2.UserId = twoer.Id;
                        journal2.BalanceAmount = twoer.Amount;
                        journal2.InAmount = amount * two;
                        journal2.Remark = "佣金收入";
                        journal2.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "佣金收入")).Id;
                        dbc.Journals.Add(journal2);

                        UserEntity threer= dbc.GetAll<UserEntity>().SingleOrDefault(u => u.Id == twoer.Recommend.RecommendId);
                        if(threer.Recommend.RecommendPath != "0")
                        {
                            threer.Amount = threer.Amount + amount * three;
                            threer.BonusAmount = threer.BonusAmount + amount * three;

                            JournalEntity journal3 = new JournalEntity();
                            journal3.UserId = threer.Id;
                            journal3.BalanceAmount = threer.Amount;
                            journal3.InAmount = amount * three;
                            journal3.Remark = "佣金收入";
                            journal3.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "佣金收入")).Id;
                            dbc.Journals.Add(journal3);
                        }
                    }
                }
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
        public async Task<UserTeamSearchResult> GetModelTeamListAsync(string mobile, long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserTeamSearchResult result = new UserTeamSearchResult();
                RecommendEntity user = await dbc.GetAll<RecommendEntity>().SingleOrDefaultAsync(r => r.User.Mobile == mobile);
                var recommends = dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false);
                if (teamLevel != null)
                {
                    if (user.RecommendMobile == "superhero" && user.RecommendPath == "1")
                    {
                        if (teamLevel == 1)
                        {
                            recommends = recommends.Where(a => a.RecommendId == user.UserId);
                        }
                        else if (teamLevel == 2)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains(user.UserId.ToString() + "-") && a.RecommendGenera == user.RecommendGenera + 2);
                        }
                        else if (teamLevel == 3)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains(user.UserId.ToString() + "-") && a.RecommendGenera == user.RecommendGenera + 3);
                        }
                    }
                    else
                    {
                        if (teamLevel == 1)
                        {
                            recommends = recommends.Where(a => a.RecommendId == user.UserId);
                        }
                        else if (teamLevel == 2)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains("-" + user.UserId.ToString() + "-") && a.RecommendGenera == user.RecommendGenera + 2);
                        }
                        else if (teamLevel == 3)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains("-" + user.UserId.ToString() + "-") && a.RecommendGenera == user.RecommendGenera + 3);
                        }
                    }
                }
                else
                {
                    if (user.RecommendMobile == "superhero" && user.RecommendGenera == 1)
                    {
                        recommends = recommends.Where(a => a.RecommendId == user.UserId ||
                     (a.RecommendPath.Contains(user.UserId.ToString() + "-") && a.RecommendGenera == user.RecommendGenera + 2) ||
                     (a.RecommendPath.Contains(user.UserId.ToString() + "-") && a.RecommendGenera == user.RecommendGenera + 3));
                    }
                    else
                    {
                        recommends = recommends.Where(a => a.RecommendId == user.UserId ||
                     (a.RecommendPath.Contains("-" + user.UserId.ToString() + "-") && a.RecommendGenera == user.RecommendGenera + 2) ||
                     (a.RecommendPath.Contains("-" + user.UserId.ToString() + "-") && a.RecommendGenera == user.RecommendGenera + 3));
                    }
                }
                if (keyword != null)
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
                var lists = recommends.ToList();
                var userResult = await recommends.OrderByDescending(a => a.User.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Users = userResult.Select(a => ToDTO(a.User)).ToArray();
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
                    if (recommend.RecommendMobile == "superhero" && recommend.RecommendPath == "1")
                    {
                        if (teamLevel == 1)
                        {
                            recommends = recommends.Where(a => a.RecommendId == userId);
                        }
                        else if (teamLevel == 2)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains(userId.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 2);
                        }
                        else if (teamLevel == 3)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains(userId.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 3);
                        }
                    }
                    else
                    {
                        if (teamLevel == 1)
                        {
                            recommends = recommends.Where(a => a.RecommendId == userId);
                        }
                        else if (teamLevel == 2)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains("-" + userId.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 2);
                        }
                        else if (teamLevel == 3)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains("-" + userId.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 3);
                        }                                                    
                    }                    
                }
                else
                {
                    if (recommend.RecommendMobile == "superhero" && recommend.RecommendGenera == 1)
                    {
                        recommends = recommends.Where(a => a.RecommendId == userId ||
                     (a.RecommendPath.Contains(userId.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 2) ||
                     (a.RecommendPath.Contains(userId.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 3));
                    }
                    else
                    {
                        recommends = recommends.Where(a => a.RecommendId == userId ||
                     (a.RecommendPath.Contains("-" + userId.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 2) ||
                     (a.RecommendPath.Contains("-" + userId.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 3));
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
                var lists = recommends.ToList();
                var userResult = await recommends.OrderByDescending(a => a.User.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Users = userResult.Select(a => ToDTO(a.User)).ToArray();
                return result;
            }
        }
    }
}
