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
            return dto;
        }

        public async Task<JournalSearchResult> GetModelListAsync(long? userId, long? journalTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                var entities = dbc.GetAll<JournalEntity>();
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
                    entities = entities.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                result.TotalInAmount = await entities.SumAsync(j => j.InAmount);
                result.TotalOutAmount = await entities.SumAsync(j => j.OutAmount);
                var journalResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
