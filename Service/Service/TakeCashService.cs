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
    public class TakeCashService : ITakeCashService
    {
        public TakeCashDTO ToDTO(TakeCashEntity entity)
        {
            TakeCashDTO dto = new TakeCashDTO();
            dto.Amount = entity.Amount;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Integral = entity.Integral;
            dto.IntegralTypeId = entity.IntegralTypeId;
            dto.IntegralTypeName = entity.IntegralType.Name;
            dto.PlatformUserId = entity.PlatformUserId;
            dto.PlatformUserMobile = entity.PlatformUser.Mobile;
            dto.StateId = entity.StateId;
            dto.StateName = entity.State.Name;
            return dto;
        }
        public async Task<TakeCashSearchResult> GetModelListAsync(long? stateId, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                TakeCashSearchResult result = new TakeCashSearchResult();
                var entities = dbc.GetAll<TakeCashEntity>();
                if(stateId != null)
                {
                    entities = entities.Where(t=>t.StateId==stateId);
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    entities = entities.Where(t => t.PlatformUser.Mobile.Contains(mobile));
                }
                if(startTime!=null)
                {
                    entities = entities.Where(t => t.CreateTime >=startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                long givingIntegralId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(i=>i.Name== "商家积分").Id;
                long useIntegralId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(i => i.Name == "消费积分").Id;
                long sId = dbc.GetAll<StateEntity>().SingleOrDefault(s=>s.Name== "已转账").Id;
                result.TotalCount = await entities.LongCountAsync();
                result.GivingIntegralCount = await entities.Where(t => t.IntegralTypeId == givingIntegralId && t.StateId== sId).SumAsync(t => t.Integral);
                result.UseIntegralCount = await entities.Where(t => t.IntegralTypeId == useIntegralId && t.StateId == sId).SumAsync(t => t.Integral);
                var res = await entities.OrderByDescending(t => t.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.TakeCashes = res.Select(t => ToDTO(t)).ToArray();
                return result;
            }
        }

        public async Task<decimal> CalcAsync(string description, long integral)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var setting = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(s => s.Description == description);
                return Convert.ToDecimal(setting.Name) * integral;
            }
        }
    }
}
