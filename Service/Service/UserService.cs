using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class UserService : IUserService
    {
        private static ILog log = LogManager.GetLogger(typeof(UserService));
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
            dto.Recommender = entity.Recommend.RecommendMobile;
            dto.HeadPic = entity.HeadPic;
            dto.ShareCode = entity.ShareCode;
            return dto;
        }

        public async Task<long> AddAsync(string mobile, string password, long levelTypeId, string recommendMobile,string nickName,string avatarUrl)
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
                        user.NickName = string.IsNullOrEmpty(nickName)?"无昵称":nickName;
                        user.HeadPic = string.IsNullOrEmpty(avatarUrl)? "/images/headpic.png" : avatarUrl;
                        dbc.Users.Add(user);
                        await dbc.SaveChangesAsync();

                        long recommendId = (await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == recommendMobile)).Id;
                        RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.UserId == recommendId);

                        if (recommend == null)
                        {
                            scope.Rollback();
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
                    catch (Exception ex)
                    {
                        scope.Rollback();
                        return -3;
                    }
                }
            }
        }

        public async Task<bool> AddAmountAsync(string mobile, decimal amount)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user= await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if(user==null)
                {
                    return false;
                }
                user.Amount = user.Amount + amount;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateInfoAsync(long id, string nickName, string headpic)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                if (nickName != null)
                {
                    entity.NickName = nickName;
                }
                if (headpic != null)
                {
                    entity.HeadPic = headpic;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateShareCodeAsync(long id, string codeUrl)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.ShareCode = codeUrl;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<long> AddRecommendAsync(long userId, string recommendMobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == userId);
                long recommendId = (await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Mobile == recommendMobile)).Id;
                RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.UserId == recommendId);
                if (user == null)
                {
                    return -1;
                }
                if (recommend == null)
                {
                    return -2;
                }
                RecommendEntity ruser = new RecommendEntity();
                ruser.UserId = userId;
                ruser.RecommendId = recommendId;
                ruser.RecommendGenera = recommend.RecommendGenera + 1;
                ruser.RecommendPath = recommend.RecommendPath + "-" + userId;

                dbc.Recommends.Add(ruser);
                await dbc.SaveChangesAsync();
                return user.Id;
            }
        }
        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                var address = dbc.GetAll<AddressEntity>().Where(a => a.UserId == id);
                if (address.LongCount() > 0)
                {
                    await address.ForEachAsync(a => a.IsDeleted = true);
                }
                var bankAccounts = dbc.GetAll<BankAccountEntity>().Where(a => a.UserId == id);
                if (bankAccounts.LongCount() > 0)
                {
                    await bankAccounts.ForEachAsync(a => a.IsDeleted = true);
                }
                RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(r => r.UserId == id);
                if (recommend != null)
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
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
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
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                if (entity.Password != CommonHelper.GetMD5(password + entity.Salt))
                {
                    return -2;
                }
                entity.Password = CommonHelper.GetMD5(newPassword + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> ResetPasswordAsync(long id, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> ResetPasswordAsync(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Mobile == mobile);
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
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Mobile == mobile);
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
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                if (entity.Password != CommonHelper.GetMD5(password + entity.Salt))
                {
                    return -2;
                }
                if (entity.IsEnabled == false)
                {
                    return -3;
                }
                return entity.Id;
            }
        }

        public async Task<long> BalancePayAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    return -1;
                }
                
                UserEntity user = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == order.BuyerId);
                if (user == null)
                {
                    return -2;
                }

                var orderlists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
                decimal totalAmount = 0;
                foreach (var orderlist in orderlists)
                {
                    GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == orderlist.GoodsId);
                    totalAmount = totalAmount + orderlist.TotalFee;
                    if (goods == null)
                    {
                        continue;
                    }
                    
                    if (goods.Inventory < orderlist.Number)
                    {
                        return -3;
                    }

                    BonusRatioEntity bonusRatio = await dbc.GetAll<BonusRatioEntity>().SingleOrDefaultAsync(b => b.GoodsId == goods.Id);
                    decimal one = 0;
                    decimal two = 0;
                    decimal three = 0;

                    UserEntity oneer = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == user.Recommend.RecommendId);
                    if (oneer != null && oneer.Recommend.RecommendPath != "0")
                    {
                        if (oneer.Level.Name == "普通会员" && bonusRatio != null)
                        {
                            one = bonusRatio.CommonOne / 100;
                        }
                        else if (oneer.Level.Name == "黄金会员" && bonusRatio != null)
                        {
                            one = bonusRatio.GoldOne / 100;
                        }
                        else if (oneer.Level.Name == "铂金会员" && bonusRatio != null)
                        {
                            one = bonusRatio.PlatinumOne / 100;
                        }

                        oneer.Amount = oneer.Amount + orderlist.TotalFee * one;
                        oneer.BonusAmount = oneer.BonusAmount + orderlist.TotalFee * one;

                        JournalEntity journal1 = new JournalEntity();
                        journal1.UserId = oneer.Id;
                        journal1.BalanceAmount = oneer.Amount;
                        journal1.InAmount = orderlist.TotalFee * one;
                        journal1.Remark = "商品佣金收入";
                        journal1.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "佣金收入")).Id;
                        journal1.OrderCode = order.Code;
                        dbc.Journals.Add(journal1);

                        UserEntity twoer = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == oneer.Recommend.RecommendId);
                        if (twoer != null && twoer.Recommend.RecommendPath != "0")
                        {
                            if (twoer.Level.Name == "普通会员" && bonusRatio != null)
                            {
                                two = bonusRatio.CommonTwo / 100;
                            }
                            else if (twoer.Level.Name == "黄金会员" && bonusRatio != null)
                            {
                                two = bonusRatio.GoldTwo / 100;
                            }
                            else if (twoer.Level.Name == "铂金会员" && bonusRatio != null)
                            {
                                two = bonusRatio.PlatinumTwo / 100;
                            }

                            twoer.Amount = twoer.Amount + orderlist.TotalFee * two;
                            twoer.BonusAmount = twoer.BonusAmount + orderlist.TotalFee * two;

                            JournalEntity journal2 = new JournalEntity();
                            journal2.UserId = twoer.Id;
                            journal2.BalanceAmount = twoer.Amount;
                            journal2.InAmount = orderlist.TotalFee * two;
                            journal2.Remark = "商品佣金收入";
                            journal2.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "佣金收入")).Id;
                            journal2.OrderCode = order.Code;
                            dbc.Journals.Add(journal2);

                            UserEntity threer = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == twoer.Recommend.RecommendId);
                            if (threer != null && threer.Recommend.RecommendPath != "0")
                            {
                                if (threer.Level.Name == "普通会员" && bonusRatio != null)
                                {
                                    three = bonusRatio.CommonThree / 100;
                                }
                                else if (threer.Level.Name == "黄金会员" && bonusRatio != null)
                                {
                                    three = bonusRatio.GoldThree / 100;
                                }
                                else if (threer.Level.Name == "铂金会员" && bonusRatio != null)
                                {
                                    three = bonusRatio.PlatinumThree / 100;
                                }

                                threer.Amount = threer.Amount + orderlist.TotalFee * three;
                                threer.BonusAmount = threer.BonusAmount + orderlist.TotalFee * three;

                                JournalEntity journal3 = new JournalEntity();
                                journal3.UserId = threer.Id;
                                journal3.BalanceAmount = threer.Amount;
                                journal3.InAmount = orderlist.TotalFee * three;
                                journal3.Remark = "商品佣金收入";
                                journal3.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "佣金收入")).Id;
                                journal3.OrderCode = order.Code;
                                dbc.Journals.Add(journal3);
                            }
                        }
                    }
                    goods.Inventory = goods.Inventory - orderlist.Number;
                    goods.SaleNum = goods.SaleNum + orderlist.Number;
                }
                decimal up1 = 0;
                decimal up2 = 0;
                decimal up3 = 0;
                SettingEntity upSetting1 = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "普通会员→黄金会员");
                SettingEntity upSetting2 = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "普通会员→→铂金会员");
                SettingEntity upSetting3 = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(i => i.Name == "黄金会员→铂金会员");
                if (upSetting1 != null)
                {
                    decimal.TryParse(upSetting1.Parm, out up1);
                }
                if (upSetting2 != null)
                {
                    decimal.TryParse(upSetting2.Parm, out up2);
                }
                if (upSetting3 != null)
                {
                    decimal.TryParse(upSetting3.Parm, out up3);
                }
                long level1 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "普通会员")).Id;
                long level2 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "黄金会员")).Id;
                long level3 = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "铂金会员")).Id;
                if (order.Amount > user.Amount)
                {
                    return -4;
                }
                long levelId = user.LevelId;
                user.Amount = user.Amount - order.Amount;
                user.BuyAmount = user.BuyAmount + order.Amount;

                order.PayTime = DateTime.Now;
                order.OrderStateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "待发货")).Id;
                if (order.Deliver== "无需物流")
                {
                    order.OrderStateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "已完成")).Id;
                }            
                if (levelId == level1)
                {
                    if (order.Amount >= up1 && order.Amount < up2)
                    {
                        user.LevelId = level2;
                    }
                    else if (order.Amount >= up2)
                    {
                        user.LevelId = level3;
                    }                    
                }
                else if (levelId == level2)
                {
                    if (order.Amount > up3)
                    {
                        user.LevelId = level3;
                    }
                }

                JournalEntity journal = new JournalEntity();
                journal.UserId = user.Id;
                journal.BalanceAmount = user.Amount;
                journal.OutAmount = order.Amount;
                journal.Remark = "购买商品";
                journal.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "购物")).Id;
                journal.OrderCode = order.Code;
                dbc.Journals.Add(journal);                
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public long WeChatPay(string code)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = dbc.GetAll<OrderEntity>().SingleOrDefault(o => o.Code == code);
                if (order == null)
                {
                    return -1;
                }

                if(order.OrderState.Name=="待发货" || order.OrderState.Name=="已完成")
                {
                    return -4;
                }

                UserEntity user = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == order.BuyerId);
                if (user == null)
                {
                    return -2;
                }

                var orderlists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
                decimal totalAmount = 0;
                foreach (var orderlist in orderlists)
                {
                    GoodsEntity goods = dbc.GetAll<GoodsEntity>().SingleOrDefault(g => g.Id == orderlist.GoodsId);
                    totalAmount = totalAmount + orderlist.TotalFee;
                    if (goods == null)
                    {
                        continue;
                    }

                    if (goods.Inventory < orderlist.Number)
                    {
                        return -3;
                    }

                    BonusRatioEntity bonusRatio = dbc.GetAll<BonusRatioEntity>().SingleOrDefault(b => b.GoodsId == goods.Id);
                    decimal one = 0;
                    decimal two = 0;
                    decimal three = 0;

                    UserEntity oneer = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == user.Recommend.RecommendId);
                    if (oneer != null && oneer.Recommend.RecommendPath != "0")
                    {
                        if (oneer.Level.Name == "普通会员" && bonusRatio != null)
                        {
                            one = bonusRatio.CommonOne / 100;
                        }
                        else if (oneer.Level.Name == "黄金会员" && bonusRatio != null)
                        {
                            one = bonusRatio.GoldOne / 100;
                        }
                        else if (oneer.Level.Name == "铂金会员" && bonusRatio != null)
                        {
                            one = bonusRatio.PlatinumOne / 100;
                        }

                        oneer.Amount = oneer.Amount + orderlist.TotalFee * one;
                        oneer.BonusAmount = oneer.BonusAmount + orderlist.TotalFee * one;

                        JournalEntity journal1 = new JournalEntity();
                        journal1.UserId = oneer.Id;
                        journal1.BalanceAmount = oneer.Amount;
                        journal1.InAmount = orderlist.TotalFee * one;
                        journal1.Remark = "商品佣金收入";
                        journal1.JournalTypeId = dbc.GetAll<IdNameEntity>().SingleOrDefault(i => i.Name == "佣金收入").Id;
                        journal1.OrderCode = order.Code;
                        dbc.Journals.Add(journal1);

                        UserEntity twoer = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == oneer.Recommend.RecommendId);
                        if (twoer != null && twoer.Recommend.RecommendPath != "0")
                        {
                            if (twoer.Level.Name == "普通会员" && bonusRatio != null)
                            {
                                two = bonusRatio.CommonTwo / 100;
                            }
                            else if (twoer.Level.Name == "黄金会员" && bonusRatio != null)
                            {
                                two = bonusRatio.GoldTwo / 100;
                            }
                            else if (twoer.Level.Name == "铂金会员" && bonusRatio != null)
                            {
                                two = bonusRatio.PlatinumTwo / 100;
                            }

                            twoer.Amount = twoer.Amount + orderlist.TotalFee * two;
                            twoer.BonusAmount = twoer.BonusAmount + orderlist.TotalFee * two;

                            JournalEntity journal2 = new JournalEntity();
                            journal2.UserId = twoer.Id;
                            journal2.BalanceAmount = twoer.Amount;
                            journal2.InAmount = orderlist.TotalFee * two;
                            journal2.Remark = "商品佣金收入";
                            journal2.JournalTypeId = dbc.GetAll<IdNameEntity>().SingleOrDefault(i => i.Name == "佣金收入").Id;
                            journal2.OrderCode = order.Code;
                            dbc.Journals.Add(journal2);

                            UserEntity threer = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefault(u => u.Id == twoer.Recommend.RecommendId);
                            if (threer != null && threer.Recommend.RecommendPath != "0")
                            {
                                if (threer.Level.Name == "普通会员" && bonusRatio != null)
                                {
                                    three = bonusRatio.CommonThree / 100;
                                }
                                else if (threer.Level.Name == "黄金会员" && bonusRatio != null)
                                {
                                    three = bonusRatio.GoldThree / 100;
                                }
                                else if (threer.Level.Name == "铂金会员" && bonusRatio != null)
                                {
                                    three = bonusRatio.PlatinumThree / 100;
                                }

                                threer.Amount = threer.Amount + orderlist.TotalFee * three;
                                threer.BonusAmount = threer.BonusAmount + orderlist.TotalFee * three;

                                JournalEntity journal3 = new JournalEntity();
                                journal3.UserId = threer.Id;
                                journal3.BalanceAmount = threer.Amount;
                                journal3.InAmount = orderlist.TotalFee * three;
                                journal3.Remark = "商品佣金收入";
                                journal3.JournalTypeId = dbc.GetAll<IdNameEntity>().SingleOrDefault(i => i.Name == "佣金收入").Id;
                                journal3.OrderCode = order.Code;
                                dbc.Journals.Add(journal3);
                            }
                        }
                    }
                    goods.Inventory = goods.Inventory - orderlist.Number;
                    goods.SaleNum = goods.SaleNum + orderlist.Number;
                }
                decimal up1 = 0;
                decimal up2 = 0;
                decimal up3 = 0;
                SettingEntity upSetting1 = dbc.GetAll<SettingEntity>().SingleOrDefault(i => i.Name == "普通会员→黄金会员");
                SettingEntity upSetting2 = dbc.GetAll<SettingEntity>().SingleOrDefault(i => i.Name == "普通会员→→铂金会员");
                SettingEntity upSetting3 = dbc.GetAll<SettingEntity>().SingleOrDefault(i => i.Name == "黄金会员→铂金会员");
                if (upSetting1 != null)
                {
                    decimal.TryParse(upSetting1.Parm, out up1);
                }
                if (upSetting2 != null)
                {
                    decimal.TryParse(upSetting2.Parm, out up2);
                }
                if (upSetting3 != null)
                {
                    decimal.TryParse(upSetting3.Parm, out up3);
                }
                long level1 = dbc.GetAll<IdNameEntity>().SingleOrDefault(i => i.Name == "普通会员").Id;
                long level2 = dbc.GetAll<IdNameEntity>().SingleOrDefault(i => i.Name == "黄金会员").Id;
                long level3 = dbc.GetAll<IdNameEntity>().SingleOrDefault(i => i.Name == "铂金会员").Id;
                //if (order.Amount > user.Amount)
                //{
                //    return -4;
                //}
                long levelId = user.LevelId;
                user.BuyAmount = user.BuyAmount + order.Amount;

                order.PayTime = DateTime.Now;
                order.OrderStateId = dbc.GetAll<IdNameEntity>().SingleOrDefault(i => i.Name == "待发货").Id;
                if (order.Deliver == "无需物流")
                {
                    order.OrderStateId = dbc.GetAll<IdNameEntity>().SingleOrDefault(i => i.Name == "已完成").Id;
                }

                if (levelId == level1)
                {
                    if (order.Amount >= up1 && order.Amount < up2)
                    {
                        user.LevelId = level2;
                    }
                    else if (order.Amount >= up2)
                    {
                        user.LevelId = level3;
                    }
                }
                else if (levelId == level2)
                {
                    if (order.Amount > up3)
                    {
                        user.LevelId = level3;
                    }
                }

                JournalEntity journal = new JournalEntity();
                journal.UserId = user.Id;
                journal.BalanceAmount = user.Amount;
                journal.OutAmount = order.Amount;
                journal.Remark = "微信支付购买商品";
                journal.JournalTypeId = dbc.GetAll<IdNameEntity>().SingleOrDefault(i => i.Name == "购物").Id;
                journal.OrderCode = order.Code;
                dbc.Journals.Add(journal);         

                dbc.SaveChanges();
                log.DebugFormat("微信支付后订单状态：{0}", order.OrderStateId);
                return 1;
            }
        }

        public async Task<CalcAmountResult> CalcCount()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CalcAmountResult res = new CalcAmountResult();
                var users = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false);
                res.TotalAmount = await users.SumAsync(u => u.Amount);
                res.TotalBonusAmount = await users.SumAsync(u => u.BonusAmount);
                res.TotalBuyAmount = await users.SumAsync(u => u.BuyAmount);
                return res;
            }
        }

        public async Task<UserDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == id);
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
                var user = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (user == null)
                {
                    return null;
                }
                return ToDTO(user);
            }
        }

        public async Task<UserSearchResult> GetModelListAsync(long? levelId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var users = dbc.GetAll<UserEntity>().Where(u => u.IsNull == false);

                if (levelId != null)
                {
                    users = users.Where(a => a.LevelId == levelId);
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
                if (string.IsNullOrEmpty(mobile))
                {
                    mobile = (await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Recommend.RecommendGenera == 1)).Mobile;
                }
                RecommendEntity user = await dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(r => r.User.Mobile == mobile);
                if (user == null)
                {
                    return result;
                }
                var recommends = dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false);
                if (teamLevel != null)
                {
                    if (user.RecommendMobile == "superhero" && user.RecommendGenera == 1)
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
                if (!string.IsNullOrEmpty(keyword))
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
                result.PageCount = (int)Math.Ceiling(recommends.LongCount() * 1.0f / pageSize);
                result.TeamLeader = ToDTO(user.User);
                var userResult = await recommends.OrderByDescending(a => a.User.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Members = userResult.Select(a => ToDTO(a.User)).ToArray();
                return result;
            }
        }
        public async Task<UserTeamSearchResult> GetModelTeamListAsync(long userId, long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserTeamSearchResult result = new UserTeamSearchResult();
                RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(r => r.UserId == userId);
                var recommends = dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false);
                if (teamLevel != null)
                {
                    if (recommend.RecommendMobile == "superhero" && recommend.RecommendGenera == 1)
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
                result.TotalCount = recommends.LongCount();
                result.PageCount = (int)Math.Ceiling(recommends.LongCount() * 1.0f / pageSize);
                var userResult = await recommends.OrderByDescending(a => a.User.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Members = userResult.Select(a => ToDTO(a.User)).ToArray();
                return result;
            }
        }
    }
}
