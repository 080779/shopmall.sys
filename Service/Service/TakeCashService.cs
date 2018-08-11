using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class TakeCashService : ITakeCashService
    {
        public TakeCashDTO ToDTO(TakeCashEntity entity, PayCodeDTO payCode, BankAccountDTO bankAccount)
        {
            TakeCashDTO dto = new TakeCashDTO();
            dto.Amount = entity.Amount;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.StateId = entity.StateId;
            dto.StateName = entity.State.Name;
            dto.PayTypeId = entity.PayTypeId;
            dto.PayTypeName = entity.PayType.Name;
            dto.PayCode = payCode;
            dto.BankAccount = bankAccount;
            dto.NickName = entity.User.NickName;
            dto.Mobile = entity.User.Mobile;
            dto.Code = entity.User.Code;
            dto.AdminMobile = entity.AdminMobile;
            return dto;
        }
        public BankAccountDTO ToDTO(BankAccountEntity entity)
        {
            BankAccountDTO dto = new BankAccountDTO();
            if(entity==null)
            {
                return dto = null;
            }
            dto.Name = entity.Name;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.BankAccount = entity.BankAccount;
            dto.BankName = entity.BankName;
            dto.Description = entity.Description;
            dto.UserId = entity.UserId;
            return dto;
        }
        public PayCodeDTO ToDTO(PayCodeEntity entity)
        {
            PayCodeDTO dto = new PayCodeDTO();
            if (entity == null)
            {
                return dto = null;
            }
            dto.Description = entity.Description;
            dto.Name = entity.Name;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.CodeUrl = entity.CodeUrl;
            dto.UserId = entity.UserId;
            return dto;
        }

        public async Task<long> AddAsync(long userId, long payTypeId, decimal amount, string descripton)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == userId);
                if(user==null)
                {
                    return -1;
                }
                if(user.Amount<amount)
                {
                    return -2;
                }
                if(user.Level.Name=="普通会员")
                {
                    return -4;
                }
                TakeCashEntity entity = new TakeCashEntity();
                entity.UserId = userId;
                entity.PayTypeId = payTypeId;
                var stateId = await dbc.GetIdAsync<IdNameEntity>(i => i.Name == "未结款");
                if(stateId == 0)
                {
                    return -3;
                }
                entity.StateId = stateId;
                entity.Amount = amount;
                entity.Description = descripton;
                dbc.TakeCashes.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }
        
        public async Task<long> Confirm(long id,long adminId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                TakeCashEntity takeCash = await dbc.GetAll<TakeCashEntity>().SingleOrDefaultAsync(t=>t.Id==id);
                if(takeCash==null)
                {
                    return -1;
                }
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == takeCash.UserId);
                if(user==null)
                {
                    return -2;
                }
                if(takeCash.Amount>user.Amount)
                {
                    return -3;
                }
                user.Amount = user.Amount - takeCash.Amount;
                takeCash.StateId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "已结款")).Id;
                takeCash.AdminMobile = (await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == adminId)).Mobile;
                JournalEntity journal = new JournalEntity();
                journal.OutAmount = takeCash.Amount;
                journal.JournalTypeId = (await dbc.GetAll<IdNameEntity>().SingleOrDefaultAsync(i => i.Name == "余额提现")).Id;
                journal.Remark = "余额提现";
                journal.UserId = takeCash.UserId;
                journal.BalanceAmount = user.Amount;
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();
                return takeCash.Id;
            }
        }

        public async Task<TakeCashSearchResult> GetModelListAsync(long? userId,long? stateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                TakeCashSearchResult result = new TakeCashSearchResult();
                var entities = dbc.GetAll<TakeCashEntity>();
                if(userId!=null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
                if (stateId != null)
                {
                    entities = entities.Where(a => a.StateId == stateId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.User.Mobile.Contains(keyword) || g.User.NickName.Contains(keyword));
                }
                if (startTime != null)
                {
                    entities = entities.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                var takeCashResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.TakeCashes = takeCashResult.Select(a => ToDTO(a,ToDTO(dbc.GetAll<PayCodeEntity>().SingleOrDefault(p => p.UserId == a.UserId)),ToDTO(dbc.GetAll<BankAccountEntity>().SingleOrDefault(b=>b.UserId==a.UserId)))).ToArray();
                return result;
            }
        }
    }
}
