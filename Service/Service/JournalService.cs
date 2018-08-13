using IMS.Common;
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
    public class JournalService : IJournalService
    {
        public JournalDTO ToDTO(JournalEntity entity)
        {
            JournalDTO dto = new JournalDTO();
            dto.BalanceAmount = entity.BalanceAmount;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.InAmount = entity.InAmount;
            dto.JournalTypeId = entity.JournalTypeId;
            dto.JournalTypeName = entity.JournalType.Name;
            dto.OutAmount = entity.OutAmount;
            dto.Remark = entity.Remark;
            dto.RemarkEn = entity.RemarkEn;
            dto.UserId = entity.UserId;
            dto.Mobile = entity.User.Mobile;
            dto.NickName = entity.User.NickName;
            dto.OrderCode = entity.OrderCode;
            dto.IsEnabled = entity.IsEnabled;
            dto.LevelId = entity.LevelId;
            dto.GoodsId = entity.GoodsId;
            return dto;
        }

        public async Task<JournalSearchResult> GetModelListAsync(long? userId, long? journalTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                var entities = dbc.GetAll<JournalEntity>().AsNoTracking().Where(j=>j.IsEnabled==true);
                if (userId != null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
                if (journalTypeId != null)
                {
                    entities = entities.Where(a => a.JournalTypeId == journalTypeId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Remark.Contains(keyword) || g.User.Mobile.Contains(keyword) || g.User.NickName.Contains(keyword) || g.OrderCode.Contains(keyword));
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
                decimal? totalInAmount = await entities.SumAsync(j => j.InAmount);
                decimal? totalOutAmount= await entities.SumAsync(j => j.OutAmount);
                result.TotalInAmount = totalInAmount == null ? 0 : totalInAmount;
                result.TotalOutAmount = totalOutAmount == null ? 0 : totalOutAmount;
                var journalResult = await entities.Include(j => j.JournalType).Include(j => j.User).OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
