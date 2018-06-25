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
    public class NoticeService : INoticeService
    {
        private NoticeDTO ToDTO(NoticeEntity entity)
        {
            NoticeDTO dto = new NoticeDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Code = entity.Code;
            dto.Content = entity.Content;
            dto.FailureTime = entity.FailureTime;
            dto.IsEnabled = entity.IsEnabled;
            dto.Url = entity.Url;
            return dto;
        }
        public async Task<long> AddAsync(string content,string url, DateTime failureTime,bool isEnabled)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NoticeEntity entity = new NoticeEntity();
                entity.Code = DateTime.Now.ToString("yyyyMMddHHmmss");
                entity.Content = content;
                entity.Url = url;
                entity.FailureTime = failureTime;
                entity.IsEnabled = isEnabled;
                dbc.Notices.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NoticeEntity entity = await dbc.GetAll<NoticeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<NoticeSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NoticeSearchResult result = new NoticeSearchResult();
                var entities = dbc.GetAll<NoticeEntity>();
                if (string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Code.Contains(keyword) || g.Content.Contains(keyword));
                }
                if (startTime != null)
                {
                    entities = entities.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => a.CreateTime.Year <= endTime.Value.Year && a.CreateTime.Month <= endTime.Value.Month && a.CreateTime.Day <= endTime.Value.Day);
                }
                result.TotalCount = entities.LongCount();
                var noticesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Notices = noticesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id,string content, string url, DateTime failureTime, bool isEnabled)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NoticeEntity entity = await dbc.GetAll<NoticeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Content = content;
                entity.Url = url;
                entity.FailureTime = failureTime;
                entity.IsEnabled = isEnabled;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
