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
            dto.StateId = entity.StateId;
            dto.StateName = entity.State.Name;
            return dto;
        }
        public async Task<TakeCashSearchResult> GetModelListAsync(long? stateId, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                TakeCashSearchResult result = new TakeCashSearchResult();
                var entities = await dbc.GetAll<TakeCashEntity>().FirstOrDefaultAsync();
                
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
